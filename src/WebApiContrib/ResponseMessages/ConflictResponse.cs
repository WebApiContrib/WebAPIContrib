using System.Net;
using System.Net.Http;

namespace WebApiContrib.ResponseMessages
{
    public class ConflictResponse : ResourceResponseBase
    {
        public ConflictResponse() : base(HttpStatusCode.Conflict)
        {
        }

        public ConflictResponse(IApiResource apiResource) : base(HttpStatusCode.Conflict, apiResource)
        {
        }
    }

    public class ConflictResponse<T> : HttpResponseMessage<T>
    {
        public ConflictResponse() : base(HttpStatusCode.Conflict)
        {
        }

        public ConflictResponse(T resource) : base(resource, HttpStatusCode.Conflict)
        {
        }
    }
}