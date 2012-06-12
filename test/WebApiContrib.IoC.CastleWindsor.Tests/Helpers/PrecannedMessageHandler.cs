using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiContrib.IoC.CastleWindsor.Tests.Helpers
{
    public class PrecannedMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _response;

        public PrecannedMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                _response.RequestMessage = request;
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(_response);
                return tcs.Task;
            }

            return null;
        }
    }
}