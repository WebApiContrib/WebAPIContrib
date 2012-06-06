using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

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

	public class ConflictResponse<T> : ResourceResponseBase<T>
	{
		public ConflictResponse(T resource, IEnumerable<MediaTypeWithQualityHeaderValue> accept, IEnumerable<MediaTypeFormatter> formatters)
			: base(HttpStatusCode.Conflict, resource, accept, formatters)
		{
		}
	}
}
