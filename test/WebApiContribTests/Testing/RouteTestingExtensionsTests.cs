using System.Collections.Generic;
using System.Web.Http;
using System.Web.Routing;
using NUnit.Framework;
using WebApiContrib.Testing;
using HttpVerbs = System.Web.Mvc.HttpVerbs;

namespace WebApiContribTests.Testing
{
    [TestFixture]
    public class RouteTestingExtensionsTests
    {
        public class SampleController : ApiController
        {
            public string Get(int id)
            {
                return "Sample Data";
            }

            public string Post(string data)
            {
                return data;
            }

            [HttpGet]
            public List<string> Mine()
            {
                return new List<string> { "My Sample 1", "My Sample 2" };
            }

            [HttpGet]
            public List<string> Filter(int type)
            {
                return new List<string>();
            }
        }

        private static class SampleRouteConfig
        {
            public static void RegisterRoutes(RouteCollection routes)
            {
                // GET /api/{resource}/{action}
                routes.MapHttpRoute(
                    name: "Web API RPC",
                    routeTemplate: "api/{controller}/{action}",
                    defaults: new {},
                    constraints: new {action = @"[A-Za-z]+", httpMethod = new HttpMethodConstraint("GET")}
                    );

                // GET|PUT|DELETE /api/{resource}/id
                routes.MapHttpRoute(
                    name: "Web API Resource",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new {},
                    constraints: new {id = @"\d+"}
                    );

                // GET /api/{resource}
                routes.MapHttpRoute(
                    name: "Web API Get All",
                    routeTemplate: "api/{controller}",
                    defaults: new {action = "Get"},
                    constraints: new {httpMethod = new HttpMethodConstraint("GET")}
                    );

                // POST /api/{resource}
                routes.MapHttpRoute(
                    name: "Web API Post",
                    routeTemplate: "api/{controller}",
                    defaults: new {action = "Post"},
                    constraints: new {httpMethod = new HttpMethodConstraint("POST")}
                    );
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            RouteTable.Routes.Clear();
            SampleRouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        [Test]
        public void ShouldMapTo_Get()
        {
            const string url = "~/api/sample/5";
            url.ShouldMapTo<SampleController>(x => x.Get(5));
        }

        [Test]
        public void ShouldMapTo_NamedAction_Get()
        {
            const string url = "~/api/sample/mine";
            url.ShouldMapTo<SampleController>(x => x.Mine());
        }

        [Test]
        public void ShouldMapTo_Post()
        {
            const string url = "~/api/sample";
            url.ShouldMapTo<SampleController>(x => x.Post(null), HttpVerbs.Post);
        }

        [Test]
        public void ShouldMapTo_NonNullableQueryStringParametersIgnored()
        {
            const string url = "~/api/sample/filter";
            url.ShouldMapTo<SampleController>(x => x.Filter(default(int)));
        }
    }
}
