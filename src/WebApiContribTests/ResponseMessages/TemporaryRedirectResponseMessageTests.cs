using System;
using System.Net;
using NUnit.Framework;
using Should;
using WebApiContrib.ResponseMessages;

namespace WebApiContribTests.ResponseMessages
{
    [TestFixture]
    public class TemporaryRedirectResponseMessageTests : HttpResponseMessageTester
    {
        [Test]
        public void Should_return_an_http_response_message_with_expected_status()
        {
            var response = new TemporaryRedirectResponse();
            AssertExpectedStatus(response);
        }

        [Test]
        public void Should_add_location_header_to_the_message_when_response_contains_a_api_resource()
        {
            var uri = new Uri("http://foobar.com");

            var response = new TemporaryRedirectResponse(uri);
            AssertExpectedStatus(response);
            response.Headers.Location.ShouldEqual(uri);
        }

        protected override HttpStatusCode? ExpectedStatusCode
        {
            get { return HttpStatusCode.TemporaryRedirect; }
        }
    }
}