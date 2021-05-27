using ContactBook.Lib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Lib.DTO
{
    public class PaginationDTO
    {
        public int count { get; set; }
        public int perPage { get; set; }
        public int CurrrentPage { get; set; }
        public IEnumerable<Contact> contacts { get; set; }
        public PaginationDTO(int count, int perPage, int CurrentPage, IEnumerable<Contact> contacts)
        {
            this.count = count;
            this.perPage = perPage;
            this.CurrrentPage = CurrrentPage;
            this.contacts = contacts;
        }

    }
}
