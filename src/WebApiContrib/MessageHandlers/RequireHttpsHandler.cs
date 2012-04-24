using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiContrib.MessageHandlers
{
    public class RequireHttpsHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                return Task<HttpResponseMessage>.Factory.StartNew(
                    () => new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("SSL is required.") });
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}