using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ContactBook.Lib.DTO;
using ContactBook.Lib.Infrastructure.Interface;
using ContactBook.Lib.Model;
using ContactBook.Lib.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContactBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _config;
        // private readonly List<MyUser> _users;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;

        public UserController(IConfiguration config, IMapper mapper, UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, ITokenGenerator tokenGenerator, IUserRepository userRepository)
        {
            _config = config;
            Account account = new Account
            {
                Cloud = _config.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = _config.GetSection("CloudinarySettings:ApiKey").Value,
                ApiSecret = _config.GetSection("CloudinarySettings:ApiSecret").Value,
            };
            _cloudinary = new Cloudinary(account);
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Login a User and display token if successfully 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto model)
        {
            var userExistInDb = await _userManager.FindByEmailAsync(model.Email);
            if (userExistInDb == null)
            {
                return NotFound("User Not Found");
            }

            var result = await _signInManager.PasswordSignInAsync(userExistInDb, model.Password, false, false);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid Credentials");
            }
            var token = await _tokenGenerator.GenerateToken(userExistInDb);
            var responseToLogin = new LoginResponseDTO
            {
                Token = token,
                Message = "Login Successful",
                Success = true
            };

            return Ok(responseToLogin);
        }

        /// <summary>
        /// only user with admin role can use this to add new Users
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("add-new")]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> AddNewUser([FromForm] UserToAddDTO model)
        {
            var result = await _userRepository.AddUser(model);
            if (!result)
            {
                return BadRequest("Something went wrong");
            }
            return Ok("User Added Successfully");
        }

        /// <summary>
        /// only user with admin role can use this to get all users within the database
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-users")]
        [Authorize(Roles = "admin")]
        public IActionResult Get()
        {
            var adminUser = _userManager.GetUsersInRoleAsync("admin").GetAwaiter().GetResult();
            var regularUser = _userManager.GetUsersInRoleAsync("regular").GetAwaiter().GetResult().ToList();
            var users = new List<AppUser>();
            users.AddRange(adminUser);
            users.AddRange(regularUser);
            var usersToReturn = _mapper.Map<List<UserToReturnDTO>>(users);
            return Ok(usersToReturn);
        }
    }




}
