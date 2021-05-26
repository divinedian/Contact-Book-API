using ContactBook.Lib.Model;

namespace ContactBook.Lib.DTO
{
    public class ContactDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}