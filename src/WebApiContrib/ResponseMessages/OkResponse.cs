using System.Net;

namespace WebApiContrib.ResponseMessages
{
    public class OkResponse : ResourceResponseBase
    {
        public OkResponse() : base(HttpStatusCode.OK)
        {
        }

        public OkResponse(IApiResource resource) : base(HttpStatusCode.OK, resource)
        {
        }
    }

    public class OkResponse<T> : ResponseBase<T>
    {
        public OkResponse() : base(HttpStatusCode.OK)
        {
        }

        public OkResponse(T resource) : base(resource, HttpStatusCode.OK)
        {
        }
    }
}