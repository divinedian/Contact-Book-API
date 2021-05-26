using ContactBook.Lib.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContactBook.Lib.Infrastructure
{
    public class ContactBookDbContext : IdentityDbContext<AppUser>
    {
        private readonly DbContextOptions _option;
        public ContactBookDbContext(DbContextOptions<ContactBookDbContext> option)
            : base(option)
        {
            _option = option;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
