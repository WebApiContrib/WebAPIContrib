using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using WebApiContrib.Routing;

namespace WebApiContribTests.Routing.Mockups
{
    [HttpRoute(UriTemplate = "api/subroute/controller")]
    public class FakeController: ApiController
    {
        [HttpRoute(UriTemplate = "api/methodlevel/route/{id}")]
        public void OperationWithOptionalRoutingParameter([OptionalRouteParameter]int id)
        {
            
        }
        
        [HttpRoute(UriTemplate = "api/methodlevel/basic")]
        public void BasicOperation()
        {
            
        }
    }
}
