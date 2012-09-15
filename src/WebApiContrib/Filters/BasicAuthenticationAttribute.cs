using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiContrib.MessageHandlers;

namespace WebApiContrib.Filters
{
    public interface IBasicAuthentication
    {
        bool Authentication(string username, string password);
    }

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
        //public BasicAuthenticationAttribute(string realm, IBasicAuthentication basicAuthentication = null)
        {
            //this.BasicAuthentication = basicAuthentication;
            this.Realm = realm;
        }

        public IBasicAuthentication BasicAuthentication { get; private set; }

        public string Realm { get; private set; }

        private BasicCredentials ParseCredentials(AuthenticationHeaderValue authHeader)
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

        public MemoryCache Cache { get { return MemoryCache.Default; } }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null &&
                actionContext.Request.Headers.Authorization.Scheme == "Basic")
            {
                var credentials = ParseCredentials(actionContext.Request.Headers.Authorization);

                if (!Cache.Contains(credentials.ToString()) && Authorize(credentials.Username, credentials.Password))
                {
                    if (!Cache.Contains(credentials.ToString()))
                    {
                        //Cache.Add(credentials.ToString(), true, new DateTimeOffset(0,0,0,0,1)));
                    }
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", Realm));
                }
            }
        }

        private bool Authorize(string username, string password)
        {
            return true;
        }
    }
}
