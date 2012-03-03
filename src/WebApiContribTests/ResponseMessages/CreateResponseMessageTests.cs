using System;
using System.Net;
using NUnit.Framework;
using Should;
using WebApiContrib.ResponseMessages;

namespace WebApiContribTests.ResponseMessages
{
    [TestFixture]
    public class CreateResponseMessageTests
    {
        [Test]
        public void Should_retrun_an_http_response_message_with_status_Created()
        {
            var response = new CreateResponse();
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
        }

        [Test]
        public void Should_add_location_header_to_the_message_when_response_contains_a_api_resource()
        {
            var apiResource = new TestResource();
            var response = new CreateResponse(apiResource);
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
            response.Headers.Location.ShouldEqual(apiResource.Self);
        }


        private class TestResource : IApiResource
        {
            public Uri Self
            {
                get { return new Uri("http://api.webapicontrib.org/test/1"); }
            }
        }

    }
}