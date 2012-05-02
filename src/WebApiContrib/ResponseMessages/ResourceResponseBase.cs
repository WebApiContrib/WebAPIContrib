using System.Net;
using System.Net.Http;

namespace WebApiContrib.ResponseMessages
{
    public abstract class ResourceResponseBase : HttpResponseMessage
    {
        protected ResourceResponseBase(HttpStatusCode httpStatusCode) : base(httpStatusCode)
        {
        }

        protected ResourceResponseBase(HttpStatusCode httpStatusCode, IApiResource resource) : base(httpStatusCode)
        {
            var location = new ResourceLocation();
            resource.SetLocation(location);
            Headers.Location = location.Location;
        }
    }
}
