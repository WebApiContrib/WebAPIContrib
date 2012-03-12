using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using ContactManager.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thinktecture.Web.Http;
using Thinktecture.Web.Http.Formatters;
using Thinktecture.Web.Http.Handlers;

namespace TestProject
{
    [TestClass]
    public class EncodingTests
    {
        [TestMethod]
        public void Post_Lots_Of_Contacts_Using_EncodingHandler_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.ServiceResolver.SetService(typeof(IRequestContentReadPolicy), new ReadAsSingleObjectPolicy());
            config.MessageHandlers.Add(new EncodingHandler());
            config.Formatters.Add(new ProtoBufFormatter());

            var formatters = new List<MediaTypeFormatter>() { new JsonMediaTypeFormatter(), new ProtoBufFormatter() };

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
            //Assert.AreEqual("Hello-back", response.Content.ReadAsStringAsync().Result);
        }
    }


    public class GreetingsController : ApiController
    {
        [HttpPost]
        public string Post([FromBody]string greeting)
        {
            return greeting + "-back";
        }
    }

    public class ContactsController : ApiController
    {
        public HttpResponseMessage Post(List<Contact> contacts)
        {
            Debug.WriteLine(String.Format("POSTed Contacts: {0}", contacts.Count));

            var response = new HttpResponseMessage()
                            {
                                StatusCode = HttpStatusCode.Created
                            };

            return response;
        }
    }
}
