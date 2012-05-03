using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using WebApiContrib.Conneg;

namespace WebApiContrib.ResponseMessages
{
    public abstract class ResourceResponseBase : HttpResponseMessage
    {
        protected ResourceResponseBase(HttpStatusCode httpStatusCode) : base(httpStatusCode)
        {
        }

        protected ResourceResponseBase(HttpStatusCode httpStatusCode, IApiResource resource) : base(httpStatusCode)
        {
            var location = new ResourceLocation();
            resource.SetLocation(location);
            Headers.Location = location.Location;
        }
    }

	public abstract class ResourceResponseBase<T> : HttpResponseMessage
	{
		protected ResourceResponseBase(HttpStatusCode httpStatusCode, T resource, IEnumerable<MediaTypeWithQualityHeaderValue> accept, IEnumerable<MediaTypeFormatter> formatters)
			: base(httpStatusCode)
		{
			var result = new DefaultContentNegotiator().Negotiate(formatters, accept);
			Content = new ObjectContent<T>(resource, result.Formatter, result.MediaType.MediaType);
		}
	}
}
