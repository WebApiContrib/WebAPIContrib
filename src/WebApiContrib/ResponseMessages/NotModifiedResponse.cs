using System;
using System.Net;
using System.Net.Http.Headers;
using WebApiContrib.Internal;

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

		public NotModifiedResponse(EntityTagHeaderValue etag)
			: base(HttpStatusCode.NotModified)
		{
			this.Headers.ETag = etag;
		}
 
	}

    public class NotModifiedResponse<T> : ResponseBase<T>
    {
        public NotModifiedResponse() : base(HttpStatusCode.NotModified)
        {
        }

        public NotModifiedResponse(T resource) : base(resource, HttpStatusCode.NotModified)
        {
        }
    }
}