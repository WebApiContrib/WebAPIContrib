using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace WebApiContrib.ResponseMessages
{
    public class CreateResponse : HttpResponseMessage
    {
         public CreateResponse() : base(HttpStatusCode.Created)
         {

         }

        public CreateResponse(IApiResource resource, HttpControllerContext controllerContext) : this()
        {
            var location = new ResourceLocation(controllerContext);
            resource.SetLocation(location);
            Headers.Location = location.Location;
        }
    }

    public class CreateResponse<T> : HttpResponseMessage<T>
    {
        public CreateResponse() : base(HttpStatusCode.Created)
        {
        }

        public CreateResponse(T resource, HttpControllerContext controllerContext) : base(resource, HttpStatusCode.Created)
        {
            if (resource is IApiResource)
            {
                var apiResource = resource as IApiResource;
                var resourceLocation = new ResourceLocation(controllerContext);
                apiResource.SetLocation(resourceLocation);
                Headers.Location = resourceLocation.Location;
            }
        }
    }
}