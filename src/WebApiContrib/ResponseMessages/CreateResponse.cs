using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

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

    public class CreateResponse<T> : ResourceResponseBase<T>
    {
        public CreateResponse(T resource, IEnumerable<MediaTypeWithQualityHeaderValue> accept, IEnumerable<MediaTypeFormatter> formatters)
            : base(HttpStatusCode.Created, resource, accept, formatters)
        {
        }
    }
}
