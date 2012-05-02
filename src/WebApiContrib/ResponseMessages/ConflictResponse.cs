using System.Net;

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
}
