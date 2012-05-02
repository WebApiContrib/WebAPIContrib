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
}
