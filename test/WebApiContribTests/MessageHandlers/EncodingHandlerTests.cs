using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using NUnit.Framework;
using WebApiContrib.Formatting;
using WebApiContrib.MessageHandlers;
using WebApiContribTests.Helpers;

namespace WebApiContribTests.MessageHandlers
{
    [TestFixture]
    public class EncodingHandlerTests
    {
        [Test, Explicit]
        public void Post_Lots_Of_Contacts_Using_EncodingHandler_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.MessageHandlers.Add(new EncodingHandler());
            config.Formatters.Add(new ProtoBufFormatter());

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

        	var request = new HttpRequestMessage();
			request.Content = new ObjectContent(typeof(List<Contact>), content, new ProtoBufFormatter(), ProtoBufFormatter.DefaultMediaType.MediaType);
        	client.PostAsync("http://anything/api/contacts", request.Content).ContinueWith(task =>
        	{
        		var response = task.Result;
				Assert.IsNotNull(response);
				Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        	});
        }
    }
}
