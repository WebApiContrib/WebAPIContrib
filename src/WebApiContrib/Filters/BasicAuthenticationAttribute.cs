using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiContrib.MessageHandlers;

namespace WebApiContrib.Filters
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        internal struct BasicCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

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

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null &&
                actionContext.Request.Headers.Authorization.Scheme == "Basic")
            {
                var credentials = ParseCredentials(actionContext.Request.Headers.Authorization);

                if (!Authorize(credentials.Username, credentials.Password))
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
