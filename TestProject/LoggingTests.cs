using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using ContactManager.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Thinktecture.Web.Http;
using Thinktecture.Web.Http.Formatters;
using Thinktecture.Web.Http.Handlers;
using Thinktecture.Web.Http.Testing;

namespace TestProject
{
    [TestClass]
    public class LoggingTests
    {
        [TestMethod]
        public void Log_Simple_Request_Test_Should_Log_Request_And_Response()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.ServiceResolver.SetService(typeof(IRequestContentReadPolicy), new ReadAsSingleObjectPolicy());

            var dummyRepository = new DummyLoggingRepository();
            config.MessageHandlers.Add(new LoggingHandler(dummyRepository));

            config.MessageHandlers.Add(new EncodingHandler());
            config.Formatters.Add(new ProtoBufFormatter());

            var formatters = new List<MediaTypeFormatter>() { new JsonMediaTypeFormatter(), new ProtoBufFormatter() };

            var server = new HttpServer(config);
            var client = new HttpClient(new EncodingHandler(server));
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new List<Contact>();
            var c = new Contact { Id = 1, Birthday = DateTime.Now.AddYears(-20) };
            content.Add(c);

            var request = new HttpRequestMessage<List<Contact>>(content, ProtoBufFormatter.DefaultMediaType, formatters);
            var response = client.PostAsync("http://anything/api/contacts", request.Content).Result;

            Assert.IsNotNull(response);

            // Note: Because content within the logging handler can be extracted as an async
            // operation, there is no guarantee that that handler is called directly after we
            // get the response. So we have a sleep to give it time to execute.
            //TODO: Find better way of ensuring all logging requests are done.

            Thread.Sleep(1000);

            Assert.AreEqual<int>(2, dummyRepository.LogMessageCount);
            Assert.IsTrue(dummyRepository.HasRequestMessageTypeBeenReceived, "No request message has been logged");
            Assert.IsTrue(dummyRepository.HasResponseMessageTypeBeenReceived, "No Response message has been received");
        }
    }
}