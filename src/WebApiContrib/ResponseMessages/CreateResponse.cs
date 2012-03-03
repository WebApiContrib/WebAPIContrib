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
            Headers.Location = resource.Self;
        }
    }
}