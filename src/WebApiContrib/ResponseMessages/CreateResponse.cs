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

    public class CreateResponse<T> : ResponseBase<T>
    {
        public CreateResponse() : base(HttpStatusCode.Created)
        {
        }

        public CreateResponse(T resource) : base(resource, HttpStatusCode.Created)
        {
        }
    }
}