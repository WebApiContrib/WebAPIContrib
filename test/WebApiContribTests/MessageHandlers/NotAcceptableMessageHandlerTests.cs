using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using NUnit.Framework;
using WebApiContrib.MessageHandlers;
using Should;
using System.Net.Http.Formatting;

namespace WebApiContribTests.MessageHandlers
{
    [TestFixture]
    public class NotAcceptableMessageHandlerTests : MessageHandlerTester
    {
	    private HttpConfiguration configuration;

		[TestFixtureSetUp]
		public void Initialize()
		{
			configuration = new HttpConfiguration();
            configuration.Services.Replace(typeof(IContentNegotiator), new DefaultContentNegotiator(excludeMatchOnTypeOnly: true));
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			configuration.Dispose();
		}

        [Test]   
        public void Should_return_NotAcceptable_when_media_type_is_not_supported()
        {
            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("weird/type"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
        }

        [Test]
        public void Should_return_OK_when_media_type_is_accepted()
        {
            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);
            
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_OK_when_type_group_is_accepted()
        {
            var customXmlFormatter = new XmlMediaTypeFormatter();
            customXmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/vnd.webapi.contrib+xml"));

            configuration.Formatters.Add(customXmlFormatter);

            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/*"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_NotAcceptable_when_type_group_is_not_accepted()
        {
            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("weird/*"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.NotAcceptable);
        }

        [Test] 
        public void Should_return_OK_when_all_media_types_is_accepted()
        {
            var customXmlFormatter = new XmlMediaTypeFormatter();
            customXmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.webapi.contrib+xml"));

            configuration.Formatters.Add(customXmlFormatter);

            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_OK_when_one_of_the_media_types_is_accepted()
        {
            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("weird/type"));
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_OK_when_one_of_the_media_types_is_a_accepted_type_group()
        {
            var customXmlFormatter = new XmlMediaTypeFormatter();
            customXmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.webapi.contrib+xml"));

            configuration.Formatters.Add(customXmlFormatter);

            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);
         
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("weird/type"));
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/*"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }

        [Test]
        public void Should_return_OK_when_one_of_the_media_types_is_all_media_types()
        {
            var customXmlFormatter = new XmlMediaTypeFormatter();
            customXmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.webapi.contrib+xml"));

            configuration.Formatters.Add(customXmlFormatter);

            var notAcceptableMessageHandler = new NotAcceptableMessageHandler(configuration);

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("weird/type"));
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

            var response = ExecuteRequest(notAcceptableMessageHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
        }
    }
}
