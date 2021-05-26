using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Lib.Model
{
    public class Contact
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhotoURl { get; set; }
        public Address Address { get; set; }
    }
}
