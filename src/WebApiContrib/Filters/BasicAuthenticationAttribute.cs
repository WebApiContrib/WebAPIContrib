using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiContrib.Internal;

namespace WebApiContrib.Filters
{
    public abstract class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        protected MemoryCache Cache { get { return MemoryCache.Default; } }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (isBasicAuthentication(actionContext))
            {
                var credentials = parseCredentials(actionContext.Request.Headers.Authorization);
                if (Cache.Contains(credentials.ToString()))
                    return;

                if (Authorize(credentials.Username, credentials.Password))
                {
                    Cache.Add(credentials.ToString(), true, DateTimeOffset.Now.AddMinutes(10));
                    return;
                }
            }

            unauthorizedResponse(actionContext);
        }

        private BasicCredentials parseCredentials(AuthenticationHeaderValue authHeader)
        {
            var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');

            return new BasicCredentials
            {
                Username = credentials[0],
                Password = credentials[1]
            };
        }

        private bool isBasicAuthentication(HttpActionContext actionContext)
        {
            return actionContext.Request.Headers.Authorization != null &&
                   actionContext.Request.Headers.Authorization.Scheme == "Basic";
        }

        private void unauthorizedResponse(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
        }

        public abstract string Realm { get; }
        public abstract bool Authorize(string username, string password);
    }
}
