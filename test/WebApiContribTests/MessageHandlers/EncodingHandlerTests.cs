using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NUnit.Framework;
using WebApiContrib;
using WebApiContrib.Formatting;
using WebApiContrib.MessageHandlers;

namespace WebApiContribTests.MessageHandlers
{
    [TestFixture]
    public class EncodingHandlerTests
    {
        [Test]
        public void Post_Lots_Of_Contacts_Using_EncodingHandler_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.ServiceResolver.SetService(typeof(IRequestContentReadPolicy), new ReadAsSingleObjectPolicy());
            config.MessageHandlers.Add(new EncodingHandler());
            config.Formatters.Add(new ProtoBufFormatter());

            var formatters = new List<MediaTypeFormatter> { new JsonMediaTypeFormatter(), new ProtoBufFormatter() };

            var server = new HttpServer(config);
            var client = new HttpClient(new EncodingHandler(server));
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-protobuf"));

            var content = new List<Contact>();
            for (int i = 0; i < 1000; i++)
            {
                var c = new Contact { Id = i };
                content.Add(c);
            }

            var request = new HttpRequestMessage<List<Contact>>(content, ProtoBufFormatter.DefaultMediaType, formatters);
            var response = client.PostAsync("http://anything/api/contacts", request.Content).Result;

            Assert.IsNotNull(response);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        }
    }

    public class Contact
    {
        public DateTime Birthday { get; set; }

        public int Id { get; set; }
    }

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