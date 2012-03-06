using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Thinktecture.Web.Http.Handlers
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Trace.TraceInformation("Begin Request: {0} {1}", request.Method, request.RequestUri);

            return base.SendAsync(request, cancellationToken);
        }
    }
}