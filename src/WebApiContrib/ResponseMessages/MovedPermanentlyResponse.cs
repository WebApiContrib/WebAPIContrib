using System;
using System.Net;

namespace WebApiContrib.ResponseMessages
{
    public class MovedPermanentlyResponse : ResourceIdentifierBase
    {
        public MovedPermanentlyResponse() : base(HttpStatusCode.MovedPermanently)
        {
        }

        public MovedPermanentlyResponse(Uri resource) : base(HttpStatusCode.MovedPermanently, resource)
        {
        }
    }
}