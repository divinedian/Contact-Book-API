using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using ContactBook.Lib.Model;
using ContactBook.Lib.DTO;

namespace ContactBook.Lib.Utilities
{
    class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            //CreateMap<AppUser, UserToReturnDTO>()
              //  .ForMember(destination => destination.,
               // option => option.ResolveUsing(x=>x.DOB.CalculateDOB()));
        }
    }

    static class MyExtension
    {
        public static int CalculateDOB(this int DOB)
        {
            var age = DateTime.Now.Year - DOB;
            return age;
        }
    }
}
