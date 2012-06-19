<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
=======
﻿using System;
using System.Net;
using System.Net.Http.Headers;
using WebApiContrib.Internal;
>>>>>>> 3e52ced9beb91eeda79c9f5c128c608a523c9108

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

	public class NotModifiedResponse<T> : ResourceResponseBase<T>
	{
		public NotModifiedResponse(T resource, IEnumerable<MediaTypeWithQualityHeaderValue> accept, IEnumerable<MediaTypeFormatter> formatters)
			: base(HttpStatusCode.NotModified, resource, accept, formatters)
		{
		}
	}
}
