using System.Net.Http.Headers;

namespace WebApiContribTests
{
    public static class StandardMediaTypeHeaderValues
    {
        public static MediaTypeHeaderValue ApplicationJson = new MediaTypeHeaderValue("application/json");
        public static MediaTypeHeaderValue TextPlain = new MediaTypeHeaderValue("text/plain");
    }
}