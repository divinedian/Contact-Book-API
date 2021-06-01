using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContactBook.Lib.Model
{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string PhotoURl { get; set; }
        [Required]
        public Address Address { get; set; }
    }
}
