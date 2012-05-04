using System.Linq;
using System.Web.Http.WebHost.Routing;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Http.WebHost.Routing;
using System.Web.UI.WebControls;
using NUnit.Framework;
using WebApiContrib.Routing;
using WebApiContrib.WebHost.Routing;

namespace WebApiContribTests.Routing
{
    /// <summary>
    /// Test fixture for the HTTP route builder component
    /// </summary>
    [TestFixture]
    public class HttpRouteTableBuilderTests
    {
        /// <summary>
        /// Tests that the HTTP route table is build correctly.
        /// </summary>
        [Test]
        public void TestBuildHttpRouteTable()
        {
            var routes = new RouteCollection();
            HttpRouteTableBuilder.BuildTable(routes,typeof(HttpRouteTableBuilderTests).Assembly);

            // Check that the HTTP route table is build correctly
            Assert.AreEqual(3,routes.Count);
            Assert.IsTrue(routes.OfType<HttpWebRoute>().Any(route => route.Url == "api/methodlevel/route/{id}"));
            Assert.IsTrue(routes.OfType<HttpWebRoute>().Any(route => route.Url == "api/subroute/controller"));
            Assert.IsTrue(routes.OfType<HttpWebRoute>().Any(route => route.Url == "api/methodlevel/basic"));

            // Find the route with optional url parameters
            var routeWithParameters = routes.OfType<HttpWebRoute>()
                .FirstOrDefault(route => route.Url == "api/methodlevel/route/{id}");
            
            // Check that the ID parameter is indeed marked as optional.
            Assert.IsNotNull(routeWithParameters);
            Assert.AreEqual(RouteParameter.Optional, routeWithParameters.Defaults["id"]);
        }
    }
}
