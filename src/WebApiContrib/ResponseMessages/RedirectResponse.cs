using System;
using System.Net;

namespace WebApiContrib.ResponseMessages
{
    public class RedirectResponse : ResourceIdentifierBase
    {
        public RedirectResponse() : base(HttpStatusCode.Redirect)
        {
        }

        public RedirectResponse(Uri resource) : base(HttpStatusCode.Redirect, resource)
        {
        }
    }
}