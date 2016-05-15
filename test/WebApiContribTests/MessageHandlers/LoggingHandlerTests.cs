using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using NUnit.Framework;
using WebApiContrib.MessageHandlers;
using WebApiContrib.Testing;
using WebApiContribTests.Helpers;

namespace WebApiContribTests.MessageHandlers
{
    [TestFixture]
    public class LoggingHandlerTests
    {
        [Test]
        public void Log_Simple_Request_Test_Should_Log_Request_And_Response()
        {
            var config = TestFactory.GetDefaultConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var dummyRepository = new DummyLoggingRepository();
            config.MessageHandlers.Add(new LoggingHandler(dummyRepository));
            config.MessageHandlers.Add(new EncodingHandler());

            var server = new HttpServer(config);
            var client = new HttpClient(new EncodingHandler(server));
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new List<Contact> { new Contact { Id = 1, Birthday = DateTime.Now.AddYears(-20) } };
            var request = new HttpRequestMessage { Content = new ObjectContent(typeof (List<Contact>), content, config.Formatters.JsonFormatter) };
            var response = client.PostAsync("http://anything/api/contacts", request.Content).Result;

            Assert.IsNotNull(response);
            Assert.AreEqual(2, dummyRepository.LogMessageCount);
            Assert.IsTrue(dummyRepository.HasRequestMessageTypeBeenReceived, "No request message has been logged");
            Assert.IsTrue(dummyRepository.HasResponseMessageTypeBeenReceived, "No Response message has been received");
        }
    }
}
