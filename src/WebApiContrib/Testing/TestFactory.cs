using System.Net.Http;
using System.Web.Http;

namespace Thinktecture.Web.Http.Testing
{
    public static class TestFactory
    {
        public static HttpRequestMessage GetDefaultRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://test/");

            return request;
        }

        public static HttpConfiguration GetDefaultConfiguration()
        {
            var httpConfig = new HttpConfiguration();

            return httpConfig;
        }

        public static HttpServer GetDefaultServer()
        {
            return new HttpServer(GetDefaultConfiguration(), new FakeHandler(req => new HttpResponseMessage()));
        }
    }
}
