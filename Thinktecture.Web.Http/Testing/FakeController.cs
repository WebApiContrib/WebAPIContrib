using System.Web.Http;

namespace Thinktecture.Web.Http.Testing
{
    public class DummyController : ApiController
    {
        public string Get()
        {
            return "OK";
        }
    }
}
