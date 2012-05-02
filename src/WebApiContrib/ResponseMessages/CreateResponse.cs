using System.Net;

namespace WebApiContrib.ResponseMessages
{
    public class CreateResponse : ResourceResponseBase
    {
         public CreateResponse() : base(HttpStatusCode.Created)
         {

         }

        public CreateResponse(IApiResource resource) : base(HttpStatusCode.Created, resource)
        {
        }
    }
}
