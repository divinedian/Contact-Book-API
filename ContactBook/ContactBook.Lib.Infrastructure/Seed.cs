using ContactBook.Lib.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Lib.Infrastructure
{
    public class Seed
    {
        private const string adminEmail = "admin@gmail.com";
        private const string adminPassword = "Admin@1234";
        private const string regularEmail = "regular@gmail.com";
        private const string regularPassword = "Regular@1234";
        public static void EnsureCreated(IApplicationBuilder app)
        {
            ContactBookDbContext ctx = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<ContactBookDbContext>();

            if (ctx.Database.GetPendingMigrations().Any())
            {
                ctx.Database.Migrate();
            }
            var userManager = app.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var userRole = app.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new string[] { "admin", "regular" };

            foreach(var role in roles)
            {
                if (!userRole.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    userRole.CreateAsync(new IdentityRole { Name = role });
                }
            }
            var admin = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();

            if (admin == null)
            {
                admin = new AppUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    PhoneNumber = "08085381090",
                    Contact = new Contact
                    {
                        Email = adminEmail,
                        FirstName = "Diana",
                        PhotoURl = "string",
                        LastName = "Ekwere",
                        Address = new Address
                        {
                            Street="SangoTedo",
                            City = "Lekki",
                            State = "Lagos",
                            Country = "Nigeria"
                        }
                    }
                };

                userManager.CreateAsync(admin, adminPassword).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(admin, "admin");

                var regular = new AppUser
                {
                    Email = regularEmail,
                    UserName = regularEmail,
                    PhoneNumber = "09033128235",
                    Contact = new Contact
                    {
                        Email = regularEmail,
                        FirstName = "Dee",
                        LastName = "Etuk",
                        PhotoURl = "string",  
                        Address = new Address
                        {
                            Street = "Igbu-Efun",
                            City = "Lekki",
                            State = "Lagos",
                            Country = "Nigeria"
                        }
                    }
                };
                userManager.CreateAsync(regular, regularPassword).GetAwaiter().GetResult();
                userManager.AddToRoleAsync(regular, "regular");
            }
        }
    }
}
