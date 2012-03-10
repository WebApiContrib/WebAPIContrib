using System;
using System.Net;
using System.Web.Http.Controllers;
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

            var response = new CreateResponse(apiResource, new HttpControllerContext());
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
            response.Headers.Location.ShouldEqual(apiResource.Location);
        }

        [Test]
        public void Should_add_content_to_message_when_its_a_typed_response_message()
        {
            var apiResource = new TestResource();
            var response = new CreateResponse<TestResource>(apiResource, new HttpControllerContext());
            response.StatusCode.ShouldEqual(HttpStatusCode.Created);
            response.Headers.Location.ShouldEqual(apiResource.Location);
            response.Content.ShouldNotBeNull();
            response.Content.ObjectType.ShouldEqual(typeof(TestResource));
        }

        private class TestResource : IApiResource
        {
            public Uri Location { get; set; }

            public void SetLocation(ResourceLocation location)
            {
                location.Set(new Uri("http://foo.com"));
                Location = location.Location;
            }
        }
    }
}