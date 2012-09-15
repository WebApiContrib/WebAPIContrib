using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApiContrib.Filters
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        internal struct BasicCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }

            public override string ToString()
            {
                return String.Format("{0}:{1}", Username, Password);
            }
        }

        public BasicAuthenticationAttribute(string realm)
        {
            this.Realm = realm;
        }

        public string Realm { get; private set; }

        private BasicCredentials ParseCredentials(AuthenticationHeaderValue authHeader)
        {
            var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');

            return new BasicCredentials
            {
                Username = credentials[0],
                Password = credentials[1]
            };
        }

        protected MemoryCache Cache { get { return MemoryCache.Default; } }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (isBasicAuthentication(actionContext))
            {
                var credentials = ParseCredentials(actionContext.Request.Headers.Authorization);
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

        public virtual bool Authorize(string username, string password)
        {
            return true;
        }
    }
}
