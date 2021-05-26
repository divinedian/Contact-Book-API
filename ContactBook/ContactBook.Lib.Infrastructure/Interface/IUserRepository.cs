using ContactBook.Lib.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook.Lib.Infrastructure.Interface
{
    public interface IUserRepository
    {
        Task<bool> AddUser(UserToAddDTO newUser);
    }
}
