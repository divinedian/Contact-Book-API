using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Lib.DTO
{
    public class UserToAddDTO
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PassWord { get; set; }
        public string Role { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
