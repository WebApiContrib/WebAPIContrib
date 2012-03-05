using System.Net;
using System.Net.Http;

namespace WebApiContrib.ResponseMessages
{
    public class CreateResponse : HttpResponseMessage
    {
         public CreateResponse()
         {
             StatusCode = HttpStatusCode.Created;
         }

        public CreateResponse(IApiResource resource) :this()
        {
            var location = new ResourceLocation();
            resource.SetLocation(location);
            Headers.Location = location.Location;
        }
    }

    public class CreateResponse<T> : HttpResponseMessage<T>
    {
        public CreateResponse() : base(HttpStatusCode.Created)
        {
        }
        public CreateResponse(T resource) : base(resource, HttpStatusCode.Created)
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