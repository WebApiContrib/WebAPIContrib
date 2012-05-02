using System.Net;
using System.Net.Http;

namespace WebApiContrib.ResponseMessages
{
    public class OkResponse : HttpResponseMessage
    {
        public OkResponse() : base(HttpStatusCode.OK)
        {
        }
    }
}
