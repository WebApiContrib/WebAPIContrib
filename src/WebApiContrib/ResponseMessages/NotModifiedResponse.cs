using System.Net;

namespace WebApiContrib.ResponseMessages
{
    public class NotModifiedResponse : ResourceResponseBase
    {
        public NotModifiedResponse() : base(HttpStatusCode.NotModified)
        {
        }

        public NotModifiedResponse(IApiResource resource) : base(HttpStatusCode.NotModified, resource)
        {
        }
    }

    public class NotModifiedResponse<T> : ResponseBase<T>
    {
        public NotModifiedResponse() : base(HttpStatusCode.NotModified)
        {
        }

        public NotModifiedResponse(T resource) : base(resource, HttpStatusCode.NotModified)
        {
        }
    }
}