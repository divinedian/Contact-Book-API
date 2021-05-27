using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contact_book_Application.Data.Interface;
using ContactBook.Lib.DTO;
using ContactBook.Lib.Infrastructure;
using ContactBook.Lib.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContactBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;
        private readonly ILogger<ContactController> _logger;
        private readonly IContactRepository _contactRepository;
        private readonly UserManager<AppUser> _userManager;

        public ContactController(ILogger<ContactController> logger, IContactRepository contactRepository, IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            Account account = new Account
            {
                Cloud = _config.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = _config.GetSection("CloudinarySettings:ApiKey").Value,
                ApiSecret = _config.GetSection("CloudinarySettings:ApiSecret").Value,
            };
            _cloudinary = new Cloudinary(account);
            _logger = logger;
            _contactRepository = contactRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// This gets all Contact and can be conducted by only admin Users
        /// </summary>
        /// <returns></returns>
        [HttpGet("all-contacts")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetContacts([FromQuery] PaginationFilter filter)
        {
            var resp = await _contactRepository.Get(filter);
            return Ok(resp);
            
        }

        /// <summary>
        /// This gets contacts by Id or by Email and anyone can perform this but must be a logged in user
        /// </summary>
        /// <param name="emailOrId"></param>
        /// <returns></returns>
        [HttpGet("{emailOrId}")]
        [Authorize(Roles = "admin,regular")]
        public async Task<IActionResult> GetByIdOrEmail(string emailOrId)
        {
            return Ok(await _contactRepository.GetByIdOrEmail(emailOrId));
        }

        /// <summary>
        /// only admins can perform this, assing of new contacts
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost("add-new")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateUser([FromForm] ContactDto contact)
        {
            var result = await _contactRepository.Create(contact);
            if (result)
            {
                return Ok("Contact Successfully Added");
            }
            else
            {
                return BadRequest("Something went wrong, try again");
            }
        }

        /// <summary>
        /// Any logged in User can update Photos  but a regular user can only upload photo for their own contact
        /// whereas, the admin can update any user's photo
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch("photo/{Id}")]
        [Authorize(Roles = "admin,regular")]
        public async Task<IActionResult> UpdatePhoto(int Id, [FromForm] PhotoToAddDto model)
        {
            var ddd = User.FindFirst(ClaimTypes.Role).Value;
            var imageUploadResult = new ImageUploadResult();

            if (ddd == "admin")
            {
                //Confirm that file was added
                var file = model.PhotoFile;
                if (file.Length <= 0)
                    return BadRequest("Invalid file size");

                using (var fs = file.OpenReadStream())
                {
                    var imageUploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, fs),
                        Transformation = new Transformation().Width(300).Height(300)
                        .Crop("fill").Gravity("face")
                    };
                    imageUploadResult = _cloudinary.Upload(imageUploadParams);
                }
            }

            else
            {
                //Get the user Id
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var contact = await _contactRepository.GetByEmail(userEmail);

                //var user = await _userManager.FindByEmailAsync(userId);
                //check if id of the photo to be updated is the user that is logged in
                if (Id != contact.Id)
                {
                    return Unauthorized();
                }
                //Confirm that file was added
                var file = model.PhotoFile;
                if (file.Length <= 0)
                    return BadRequest("Invalid file size");

                using (var fs = file.OpenReadStream())
                {
                    var imageUploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, fs),
                        Transformation = new Transformation().Width(300).Height(300)
                        .Crop("fill").Gravity("face")
                    };
                    imageUploadResult = _cloudinary.Upload(imageUploadParams);
                }
            }

            var publicId = imageUploadResult.PublicId;
            var Url = imageUploadResult.Url.ToString();

            //You can send this to the database
            await _contactRepository.UpdatePhoto(Id, Url);


            return Ok(new { id = publicId, Url });


            /*if (Id != contact.Id)
            {
                return BadRequest();
            }

            await _contactRepository.Update(contact);
            return NoContent();*/
        }

        /// <summary>
        /// Only admin can delete a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var contactToDelete = await _contactRepository.GetById(id);
            if (contactToDelete == null)
            {
                return NotFound();
            }
            await _contactRepository.Delete(contactToDelete);
            return NoContent();
        }

        /// <summary>
        /// Only admin can update other user's contact fields
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPut("update/{Id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int Id, [FromForm] ContactDto contact)
        {
            var foundContact = _contactRepository.GetById(Id).GetAwaiter().GetResult();
            if (foundContact == null)
            {
                return NotFound("No such contact found");
            }

            var result = await _contactRepository.Update(Id, contact);

            if (result)
            {
                return Ok("Contact Successfully Updated");
            }
            else
            {
                return BadRequest("Something went wrong, try again");
            }
        }

        /// <summary>
        /// all logged in user can search for an existing user by name, city or state
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize(Roles = "admin,regular")]
        public IActionResult Search([FromQuery] SearchDTO model)
        {
            var contactToReturn = _contactRepository.Search(model.Name, model.State, model.City);
            if (contactToReturn == null)
            {
                return NotFound("No Contact associated search ");
            }
            var output = new List<SearchResponseDTO>();
            foreach (var contact in contactToReturn)
            {
                var response = new SearchResponseDTO
                {
                    Name = $"{contact.FirstName} {contact.LastName}",
                    Email = contact.Email,
                    Address = $"{contact.Address.Street} {contact.Address.City}, {contact.Address.State}, {contact.Address.Country}"
                };
                output.Add(response);
            }
            return Ok(output);
        }
    }
}
