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

    public abstract class ResponseBase<T> : HttpResponseMessage<T>
    {
        protected ResponseBase(HttpStatusCode httpStatusCode) : base(httpStatusCode)
        {
        }

        protected ResponseBase(T resource, HttpStatusCode httpStatusCode) : base(resource, httpStatusCode)
        {
            if (resource is IApiResource)
            {
                var apiResource = resource as IApiResource;
                var resourceLocation = new ResourceLocation();
                apiResource.SetLocation(resourceLocation);
                Headers.Location = resourceLocation.Location;
            }
        }
    }
}