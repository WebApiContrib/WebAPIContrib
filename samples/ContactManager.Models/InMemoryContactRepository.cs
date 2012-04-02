using System;
using System.Collections.Generic;
using System.Linq;

namespace ContactManager.Models
{
    public class InMemoryContactRepository : IContactRepository
    {
        private int nextContactId;

        private readonly IList<Contact> contacts;

        public InMemoryContactRepository()
        {
            contacts = new List<Contact>();

            contacts.Add(new Contact { Id = 1, Name = "Ingo Rammer", Address = "Rammer Way", City = "Landshut", State = "N/A", Zip = "12345", Email = "ingo.rammer@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });
            contacts.Add(new Contact { Id = 2, Name = "Christian Weyer", Address = "Wire Way", City = "Neustadt", State = "N/A", Zip = "23456", Email = "christian.weyer@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });
            contacts.Add(new Contact { Id = 3, Name = "Dominick Baier", Address = "Bavarian Alley", City = "Heidelberg", State = "N/A", Zip = "34567", Email = "dominick.baier@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });
            contacts.Add(new Contact { Id = 4, Name = "Christian Nagel", Address = "Nail Road", City = "Wien", State = "N/A", Zip = "45678", Email = "christian.nagel@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });
            contacts.Add(new Contact { Id = 5, Name = "Jörg Neumann", Address = "Newman Way", City = "Hamburg", State = "N/A", Zip = "56789", Email = "joerg.neumann@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });
            contacts.Add(new Contact { Id = 6, Name = "Oliver Sturm", Address = "Storm Avenue", City = "Somewhere in Scotland", State = "N/A", Zip = "67890", Email = "oliver.sturm@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });
            contacts.Add(new Contact { Id = 7, Name = "Richard Blewett", Address = "Blewett Way", City = "Somewhere in England", State = "N/A", Zip = "78901", Email = "richard.blewett@thinktecture.com", Birthday = new DateTime(1980, 1, 1) });

            nextContactId = contacts.Count + 1;
        }

        public void Update(Contact updatedContact)
        {
            var contact = Get(updatedContact.Id);
            contact.Name = updatedContact.Name;
            contact.Address = updatedContact.Address;
            contact.City = updatedContact.City;
            contact.State = updatedContact.State;
            contact.Zip = updatedContact.Zip;
            contact.Email = updatedContact.Email;
            contact.Twitter = updatedContact.Twitter;
        }

        public Contact Get(int id)
        {
            return contacts.SingleOrDefault(c => c.Id == id);
        }

        public List<Contact> GetAll()
        {
            return contacts.ToList();
        }

        public void Post(Contact contact)
        {
            contact.Id = nextContactId++;
            contacts.Add(contact);
        }

        public void Delete(int id)
        {
            var contact = Get(id);
            contacts.Remove(contact);
        }
    }
}