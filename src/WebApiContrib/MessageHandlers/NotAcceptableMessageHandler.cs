using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApiContrib.MessageHandlers
{
    public class NotAcceptableMessageHandler : DelegatingHandler
    {
        private const string allMediaTypesRange = "*/*";

    	private readonly HttpConfiguration configuration;

		public NotAcceptableMessageHandler(HttpConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException("configuration");

			this.configuration = configuration;
		}

		public NotAcceptableMessageHandler(HttpConfiguration configuration, HttpMessageHandler innerHandler)
			: base(innerHandler)
		{
			if (configuration == null)
				throw new ArgumentNullException("configuration");

			this.configuration = configuration;
		}

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!IsRequestedMediaTypeAccepted(request))
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.NotAcceptable));

            return base.SendAsync(request, cancellationToken);
        }

        private bool IsRequestedMediaTypeAccepted(HttpRequestMessage request)
        {
            var acceptHeader = request.Headers.Accept;

            return configuration
                .Formatters
                .Any(formatter => acceptHeader.Any(mediaType => formatter.SupportedMediaTypes.Contains(mediaType)) //eg text/html is requested, text/html is a supported type
                    || formatter.MediaTypeMappings.Any(m=> m.TryMatchMediaType(request) > 0)); // eg text/* is requested and text/* -> text/html is a media type mapping
        }
    }
}
