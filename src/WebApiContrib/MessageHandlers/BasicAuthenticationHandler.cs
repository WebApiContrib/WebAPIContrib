using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiContrib.Internal;

namespace WebApiContrib.MessageHandlers
{
    public abstract class BasicAuthenticationHandler : DelegatingHandler
    {
        protected abstract bool Authorize(string username, string password);

        protected abstract string Realm { get; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == "Basic")
            {
                var credentials = ParseCredentials(request.Headers.Authorization);

                if (Authorize(credentials.Username, credentials.Password))
                {
                    return base.SendAsync(request, cancellationToken);
                }
            }

            return Task<HttpResponseMessage>.Factory.StartNew(
                () =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

                    response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));

                    return response;
                });
        }

        private static BasicCredentials ParseCredentials(AuthenticationHeaderValue authHeader)
        {
            try
            {
                var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader.ToString().Substring(6))).Split(':');

                return new BasicCredentials
                {
                    Username = credentials[0],
                    Password = credentials[1]
                };
            }
            catch { }

            return new BasicCredentials();
        }
    }
}