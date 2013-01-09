using System.Collections.Generic;
using System.Web.Http;
using System.Web.Routing;
using NUnit.Framework;
using WebApiContrib.Testing;

namespace WebApiContribTests.Testing
{
    [TestFixture]
    public class RouteTestingExtensionsTests
    {
        public class SampleController : ApiController
        {
            public string Get()
            {
                return string.Empty;
            }

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
        
        public class FoobarController : ApiController
        {
            public List<int> Get()
            {
                return new List<int>();
            }

            public List<int> Get(int id)
            {
                return new List<int>();
            }
        }

        public class RestfulController : ApiController
        {
            public List<int> Get()
            {
                return new List<int>();
            }

            public List<int> Get(int id)
            {
                return new List<int>();
            }
        }

        private static class SampleRouteConfig
        {
            public static void RegisterRoutes(RouteCollection routes)
            {
                // GET|PUT|DELETE /api/foobar/{id}
                routes.MapHttpRoute(
                    name: "Restful Web API",
                    routeTemplate: "api/restful/{id}",
                    defaults: new {id = RouteParameter.Optional}
                    );

                // GET /api/{resource}/{action}
                routes.MapHttpRoute(
                    name: "Web API RPC",
                    routeTemplate: "api/{controller}/{action}",
                    defaults: new {},
                    constraints: new {action = @"[A-Za-z]+", httpMethod = new HttpMethodConstraint("GET")}
                    );

                // GET|PUT|DELETE /api/{resource}/{id}
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
        public void ShouldMapTo_WithDefaultAction()
        {
            const string url = "~/api/sample/";
            url.ShouldMapTo<SampleController>(x => x.Get());
        }

        [Test]
        public void ShouldMapTo_WithRequiredParameter()
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
            url.ShouldMapTo<SampleController>(x => x.Post(null), "POST");
        }

        [Test]
        [ExpectedException(typeof(WebApiContrib.Testing.AssertionException))]
        public void ShouldMapTo_ThrowsException_WhenControllerIsIncorrect()
        {
            const string url = "~/api/foobar";
            url.ShouldMapTo<SampleController>(x => x.Get());
        }

        [Test]
        [ExpectedException(typeof(WebApiContrib.Testing.AssertionException))]
        public void ShouldMapTo_ThrowsException_WhenActionIsIncorrect()
        {
            const string url = "~/api/sample";
            url.ShouldMapTo<SampleController>(x => x.Get(), "POST");
        }

        [Test]
        [ExpectedException(typeof(WebApiContrib.Testing.AssertionException))]
        public void ShouldMapTo_ThrowsException_WhenRequiredParameterIsMissing()
        {
            const string url = "~/api/sample";
            url.ShouldMapTo<SampleController>(x => x.Post("abc"), "POST");
        }

        [Test]
        [ExpectedException(typeof(WebApiContrib.Testing.AssertionException))]
        public void ShouldMapTo_ThrowsException_WhenParameterIsIncorrect()
        {
            const string url = "~/api/sample/30";
            url.ShouldMapTo<SampleController>(x => x.Get(15));
        }

        [Test]
        public void ShouldMapTo_AllowsOptionalParametersToBeMissing()
        {
            const string url = "~/api/foobar";
            url.ShouldMapTo<FoobarController>(x => x.Get());
        }

        [Test]
        public void ShouldMapTo_AllowsOptionalParametersToBeSpecified()
        {
            const string url = "~/api/foobar/20";
            url.ShouldMapTo<FoobarController>(x => x.Get(20));
        }

        [Test]
        [ExpectedException(typeof(WebApiContrib.Testing.AssertionException))]
        public void ShouldMapTo_ThrowsExceptionWhenOptionalParameterIsIncorrect()
        {
            const string url = "~/api/foobar/10";
            url.ShouldMapTo<FoobarController>(x => x.Get(20));
        }

        [Test]
        public void ShouldMapTo_NonNullableQueryStringParametersIgnored()
        {
            const string url = "~/api/sample/filter";
            url.ShouldMapTo<SampleController>(x => x.Filter(default(int)));
        }
    }
}
