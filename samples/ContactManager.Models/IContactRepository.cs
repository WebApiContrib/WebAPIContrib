using System.Collections.Generic;

namespace ContactManager.Models
{
    public interface IContactRepository
    {
        void Update(Contact updatedContact);

        Contact Get(int id);

        List<Contact> GetAll();

        void Post(Contact contact);

        void Delete(int id);
    }
}