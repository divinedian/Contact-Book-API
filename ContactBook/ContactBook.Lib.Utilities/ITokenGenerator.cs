using ContactBook.Lib.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Lib.Utilities
{
    public interface ITokenGenerator
    {
        Task<string> GenerateToken(AppUser user);
    }
}
