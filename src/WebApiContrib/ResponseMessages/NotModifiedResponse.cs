using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

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

	public class NotModifiedResponse<T> : ResourceResponseBase<T>
	{
		public NotModifiedResponse(T resource, IEnumerable<MediaTypeWithQualityHeaderValue> accept, IEnumerable<MediaTypeFormatter> formatters)
			: base(HttpStatusCode.NotModified, resource, accept, formatters)
		{
		}
	}
}
