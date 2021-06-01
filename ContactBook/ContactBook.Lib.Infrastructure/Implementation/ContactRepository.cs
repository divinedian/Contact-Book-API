using Contact_book_Application.Data.Interface;
using ContactBook.Lib.DTO;
using ContactBook.Lib.Infrastructure;
using ContactBook.Lib.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Implementation
{
    public class ContactRepository : IContactRepository
    {
        private ContactBookDbContext _ctx;
        public ContactRepository(ContactBookDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> Create(ContactDto contact)
        {
            var newContact = new Contact()

            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Address = new Address
                {
                    Street = contact.Street,
                    City = contact.City,
                    State = contact.State,
                    Country = contact.Country
                }
            };
            
            var result = await _ctx.AddAsync(newContact);
            return _ctx.SaveChanges()>0;            
        }

        public async Task Delete(Contact contact)
        {
            var address = await _ctx.Addresses.Where(x => x.Id == contact.Address.Id).FirstOrDefaultAsync();
            _ctx.Remove(contact);
            _ctx.Remove(address);
            await _ctx.SaveChangesAsync();
        }

        public async Task<List<ContactDto>> Get(PaginationFilter filter)
        {
            List<ContactDto> contactsToGet = new List<ContactDto>();
            var validPagesFilter = new PaginationFilter(filter.CurrentPage);

            var resp = await _ctx.Contacts.Include(x => x.Address)
                .Skip((validPagesFilter.CurrentPage-1)*4)
                .Take(4).ToListAsync();
            var num = resp.Count();

            for(int i=0; i<num; i++)
            {
                ContactDto contactDto = new ContactDto()
                {
                    FirstName = resp[i].FirstName,
                    LastName = resp[i].LastName,
                    Email = resp[i].Email,
                    Street = resp[i].Address.Street,
                    City = resp[i].Address.City,
                    State = resp[i].Address.State,
                    Country = resp[i].Address.Country
                };
                contactsToGet.Add(contactDto);
            }
            return contactsToGet;
        }

        public async Task<Contact> GetByIdOrEmail(string emailorId)
        {
            if (emailorId.Contains('@'))
            {
                var contact = await _ctx.Contacts.Include(x => x.Address).Where(x => x.Email == emailorId).SingleOrDefaultAsync();
                if (contact == null)
                {
                    throw new Exception("User does not exist");
                }
                return contact;
            }
            else
            {
                var IdConverted = Convert.ToInt32(emailorId.ToString());
                var contact = await _ctx.Contacts.Include(x => x.Address).Where(x => x.Id == IdConverted).SingleOrDefaultAsync();

                if (contact == null)
                {
                    throw new Exception("User does not exist");
                }
                return contact;
            }
        }

        public async Task<Contact> GetById(int Id)
        {
            var contact = await _ctx.Contacts.Include(x => x.Address).Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (contact == null)
            {
                throw new Exception("User does not exist");
            }
            return contact;
        }

        public async Task<Contact> GetByEmail(string email)
        {
            var contact = await _ctx.Contacts.Include(x => x.Address).Where(x => x.Email == email).FirstOrDefaultAsync();
            if (contact == null)
            {
                throw new Exception("User does not exist");
            }
            return contact;
        }

        public async Task<bool> Update(int Id, ContactDto contact)
        {
            var foundContact = await GetById(Id);
            foundContact.FirstName = contact.FirstName;
            foundContact.LastName = contact.LastName;
            foundContact.Email = contact.Email;
            foundContact.Address.Street = contact.Street;
            foundContact.Address.City = contact.City;
            foundContact.Address.State = contact.State;
            foundContact.Address.Country = contact.Country;

            _ctx.Update(foundContact);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task UpdatePhoto(int Id, string photoUrl)
        {
            var foundcontact = GetById(Id).GetAwaiter().GetResult();
            foundcontact.PhotoURl = photoUrl;
            _ctx.Update(foundcontact);
            await _ctx.SaveChangesAsync();

        }

        public IQueryable<Contact> Search(string name, string state, string city)
        {
            IQueryable<Contact> query = _ctx.Contacts.Include(x => x.Address);
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.FirstName == name || e.LastName == name);
            }
            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(e => e.Address.State.Contains(state));
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(e => e.Address.City.Contains(city));
            }
            return query;
        }
    }
}
