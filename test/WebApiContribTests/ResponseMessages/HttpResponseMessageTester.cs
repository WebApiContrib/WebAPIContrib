using System;
using System.Net;
using System.Net.Http;
using Should;
using WebApiContrib.ResponseMessages;

namespace WebApiContribTests.ResponseMessages
{
    public abstract class HttpResponseMessageTester
    {
        protected abstract HttpStatusCode? ExpectedStatusCode { get; }

        internal class TestResource : IApiResource
        {
            public Uri Location { get; set; }

            public void SetLocation(ResourceLocation location)
            {
                location.Set(new Uri("http://foo.com"));
                Location = location.Location;
            }
        }

        public void AssertExpectedStatus(HttpResponseMessage response)
        {
            response.StatusCode.ShouldEqual(ExpectedStatusCode.Value);
        }
    }
}