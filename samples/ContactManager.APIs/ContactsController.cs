using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using ContactManager.Models;
using WebApiContrib.Filters;

namespace ContactManager.APIs
{
    // NOTE: Maybe it is not a good idea to expose the 'model' in the service/API.
    // Consider using use-case-based DTOs (http://davybrion.com/blog/2012/02/dtos-should-transfer-data-not-entities).
    public class ContactsController : ApiController
    {        
        private readonly IContactRepository repository;

        public ContactsController(IContactRepository repository)
        {            
            this.repository = repository;
        }

        [EnableCors]
        public IQueryable<Contact> Get()
        {
            return repository.GetAll().AsQueryable();
        }

        [EnableCors]
        public HttpResponseMessage Get(int id, HttpRequestMessage request)
        {
            var contact = repository.Get(id);

            if (contact == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Contact not found.")
                };

                throw new HttpResponseException(response);
            }

        	var contactResponse = request.CreateResponse(HttpStatusCode.OK, contact);

            contactResponse.Content.Headers.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(300));

            return contactResponse;
        }

        public HttpResponseMessage Post(Contact contact)
        {            
            repository.Post(contact);

        	var response = Request.CreateResponse(HttpStatusCode.Created, contact);

            response.Headers.Location = new Uri(ControllerContext.Request.RequestUri.LocalPath + "/" + contact.Id.ToString(CultureInfo.InvariantCulture), UriKind.Relative);
            
            return response;
        }

        public Contact Put(int id, Contact contact)
        {
            repository.Get(id);
            repository.Update(contact);

            return contact;
        }

        public Contact Delete(int id)
        {
            var deleted = repository.Get(id);
            repository.Delete(id);

            return deleted;
        }
    }
}
