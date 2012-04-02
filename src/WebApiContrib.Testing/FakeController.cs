using System.Web.Http;

namespace WebApiContrib.Testing
{
    public class DummyController : ApiController
    {
        public string Get()
        {
            return "OK";
        }
    }
}
