using AutoMapper;
using Contact_book_Application.Data.Interface;
using ContactBook.Implementation;
using ContactBook.Lib.Infrastructure;
using ContactBook.Lib.Infrastructure.Implementation;
using ContactBook.Lib.Infrastructure.Interface;
using ContactBook.Lib.Model;
using ContactBook.Lib.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactBook
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddDbContext<ContactBookDbContext>(option => option.UseSqlite(Configuration.GetConnectionString("Connection")));
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<ContactBookDbContext>();
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
            }
            ).AddXmlDataContractSerializerFormatters();

            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = false
                };
            });
            services.AddSwaggerGen(
                option =>
                {
                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        BearerFormat = "JWT",
                        Description = "Bearer authentication using JWT",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        Name = "Authorization"
                    });

                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[]{}
                        }
                    });

                    option.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "ContactBook",
                        Version = "v1"
                    });
                });
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(option => option.SwaggerEndpoint("/swagger/v1/swagger.json", "ContactBook"));
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            Seed seed = new Seed(Configuration);
            seed.EnsureCreated(app);
        }
    }
}
