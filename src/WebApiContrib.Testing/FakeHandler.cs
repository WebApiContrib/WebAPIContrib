using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.Testing
{
    public class FakeHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> f;

        public FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> f)
        {
            this.f = f;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => f(request));
        }
    }
}
