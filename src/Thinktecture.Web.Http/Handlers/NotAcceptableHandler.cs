using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Thinktecture.Web.Http.Handlers
{
    // Code based on: http://pedroreys.com/2012/02/17/extending-asp-net-web-api-content-negotiation/
    public class NotAcceptableHandler : DelegatingHandler
    {
        private const string allMediaTypesRange = "*/*";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var acceptHeader = request.Headers.Accept;

            if (!acceptHeader.Any(x => x.MediaType == allMediaTypesRange))
            {
                var hasFormatterForRequestedMediaType = GlobalConfiguration
                                    .Configuration
                                    .Formatters
                                    .Any(formatter => acceptHeader.Any(mediaType => formatter.SupportedMediaTypes.Contains(mediaType)));

                if (!hasFormatterForRequestedMediaType)
                {
                    return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.NotAcceptable));
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
