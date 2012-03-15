using System.Net;
using NUnit.Framework;
using Should;
using WebApiContrib.ResponseMessages;

namespace WebApiContribTests.ResponseMessages
{
    [TestFixture]
    public class OkResponseMessageTests : HttpResponseMessageTester
    {
        [Test]
        public void Should_return_an_http_response_message_with_expected_status()
        {
            var response = new OkResponse();

            AssertExpectedStatus(response);
        }

        [Test]
        public void Should_add_content_to_message_when_its_a_typed_response_message()
        {
            var apiResource = new TestResource();
            var response = new OkResponse<TestResource>(apiResource);

            AssertExpectedStatus(response);
            // http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.30
            // The Location response-header field is used to redirect the recipient 
            // to a location other than the Request-URI for completion of the request 
            // or identification of a new resource.
            response.Headers.Location.ShouldBeNull();
            response.Content.ShouldNotBeNull();
            response.Content.ObjectType.ShouldEqual(typeof(TestResource));
        }

        protected override HttpStatusCode? ExpectedStatusCode
        {
            get { return HttpStatusCode.OK; }
        }
    }
}