using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiContribTests.Helpers
{
    public class ContactsController : ApiController
    {
        public HttpResponseMessage Post(List<Contact> contacts)
        {
            Debug.WriteLine(String.Format("POSTed Contacts: {0}", contacts.Count));

            var response = new HttpResponseMessage
                               {
                                   StatusCode = HttpStatusCode.Created
                               };

            return response;
        }
    }
}