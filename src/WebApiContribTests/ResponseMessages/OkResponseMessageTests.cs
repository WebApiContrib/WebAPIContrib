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
        public void Should_return_an_http_response_message_with_status_Found()
        {
            var response = new OkResponse();

            AssertExpectedStatus(response);
        }

        [Test]
        public void Should_add_location_header_to_the_message_when_response_contains_a_api_resource()
        {
            var apiResource = new TestResource();
            var response = new OkResponse<TestResource>(apiResource);

            AssertExpectedStatus(response);
            response.Headers.Location.ShouldEqual(apiResource.Location);
        }

        [Test]
        public void Should_add_content_to_message_when_its_a_typed_response_message()
        {
            var apiResource = new TestResource();
            var response = new OkResponse<TestResource>(apiResource);

            AssertExpectedStatus(response);
            response.Headers.Location.ShouldEqual(apiResource.Location);
            response.Content.ShouldNotBeNull();
            response.Content.ObjectType.ShouldEqual(typeof(TestResource));
        }

        protected override HttpStatusCode? ExpectedStatusCode
        {
            get { return HttpStatusCode.OK; }
        }
    }
}