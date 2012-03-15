using System;
using System.Net;
using NUnit.Framework;
using Should;
using WebApiContrib.ResponseMessages;

namespace WebApiContribTests.ResponseMessages
{
    [TestFixture]
    public class MovedPermanentlyResponseMessageTests : HttpResponseMessageTester
    {
        [Test]
        public void Should_retrun_an_http_response_message_with_status_Created()
        {
            var response = new MovedPermanentlyResponse();
            AssertExpectedStatus(response);
        }

        [Test]
        public void Should_add_location_header_to_the_message_when_response_contains_a_api_resource()
        {
            var uri = new Uri("http://foo.com");

            var response = new MovedPermanentlyResponse(uri);
            AssertExpectedStatus(response);
            response.Headers.Location.ShouldEqual(uri);
        }

        protected override HttpStatusCode? ExpectedStatusCode
        {
            get { return HttpStatusCode.MovedPermanently; }
        }
    }
}