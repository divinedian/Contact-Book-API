using ContactBook.Lib.DTO;
using ContactBook.Lib.Infrastructure.Interface;
using ContactBook.Lib.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Lib.Infrastructure.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> AddUser(UserToAddDTO newUser)
        {
            var user = new AppUser
            {
                Email = newUser.Email,
                UserName = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                Contact = new Contact
                {
                    Email = newUser.Email,
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Address = new Address
                    {
                        Street = newUser.Street,
                        City = newUser.City,
                        State = newUser.State,
                        Country = newUser.Country
                    }
                }
            };
            await _userManager.CreateAsync(user, newUser.PassWord);
            var result = await _userManager.AddToRoleAsync(user, newUser.Role);
            return result.Succeeded;
        }
    }
}
