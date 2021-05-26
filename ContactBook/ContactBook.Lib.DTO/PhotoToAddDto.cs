using Microsoft.AspNetCore.Http;
using System;

namespace ContactBook.Lib.DTO
{
    public class PhotoToAddDto
    {
        public IFormFile PhotoFile { get; set; }
        //public DateTime DateCreated { get; set; } = DateTime.Now;

    }
}
