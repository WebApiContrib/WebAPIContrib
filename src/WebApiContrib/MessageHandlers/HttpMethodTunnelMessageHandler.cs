using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiContrib.MessageHandlers
{
    public class HttpMethodTunnelMessageHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetOverrideMethodIfAny();
            return base.SendAsync(request, cancellationToken);
        }
    }

    public static class HttpRequestMessageExtensions
    {
        public static void SetOverrideMethodIfAny(this HttpRequestMessage request)
        {
            var overrideMethod = request.GetOverrideMethod();
            if (overrideMethod != null &&
                overrideMethod != HttpMethod.Get &&
                overrideMethod != HttpMethod.Post &&
                request.Method.Safe() == overrideMethod.Safe())
            {
                request.Method = overrideMethod;
            }
        }

        public static HttpMethod GetOverrideMethod(this HttpRequestMessage request)
        {
            var method = HttpUtility.ParseQueryString(request.RequestUri.Query)["_method"];

            if (String.IsNullOrEmpty(method))
                method = request.Headers
                    .Where(h => h.Key == "X-HTTP-Method-Override")
                    .SelectMany(h => h.Value)
                    .FirstOrDefault();

            return MapMethod(method);
        }

        private static HttpMethod MapMethod(string method)
        {
            if (String.IsNullOrEmpty(method)) return null;
            if (String.Compare(method, HttpMethod.Put.Method, StringComparison.OrdinalIgnoreCase) == 0)
                return HttpMethod.Put;
            if (String.Compare(method, HttpMethod.Delete.Method, StringComparison.OrdinalIgnoreCase) == 0)
                return HttpMethod.Delete;
            if (String.Compare(method, HttpMethod.Options.Method, StringComparison.OrdinalIgnoreCase) == 0)
                return HttpMethod.Options;
            if (String.Compare(method, HttpMethod.Head.Method, StringComparison.OrdinalIgnoreCase) == 0)
                return HttpMethod.Head;
            return null;
        }
    }

    public static class HttpMethodExtensions
    {
        public static bool Safe(this HttpMethod method)
        {
            return method == HttpMethod.Get ||
                   method == HttpMethod.Head ||
                   method == HttpMethod.Options;
        }
    }
}
