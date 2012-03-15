using System.Net;
using System.Net.Http;

namespace WebApiContrib.ResponseMessages
{
    public class OkResponse : HttpResponseMessage
    {
        public OkResponse() : base(HttpStatusCode.OK)
        {
        }
    }

    public class OkResponse<T> : HttpResponseMessage<T>
    {
        public OkResponse() : base(HttpStatusCode.OK)
        {
        }

        public OkResponse(T resource) : base(resource, HttpStatusCode.OK)
        {
        }
    }
}