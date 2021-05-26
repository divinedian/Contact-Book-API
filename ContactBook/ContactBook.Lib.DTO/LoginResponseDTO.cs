using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Lib.DTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
