using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContactBook.Lib.Model
{
    public class AppUser :IdentityUser
    {
        public Contact Contact { get; set; }
    }
}
