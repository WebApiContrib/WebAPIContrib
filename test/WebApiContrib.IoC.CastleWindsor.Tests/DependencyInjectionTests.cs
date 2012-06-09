using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using WebApiContrib.IoC.CastleWindsor;
using WebApiContrib.IoC.CastleWindsor.Tests.Helpers;

namespace WebApiContrib.IoC.CastleWindsor.Tests.IoC
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void WindsorResolver_Resolves_Registered_ContactRepository_Test()
        {
            using (var container = new WindsorContainer())
            {
                container.Register(
                    Component.For<IContactRepository>().Instance(new InMemoryContactRepository()));

                var resolver = new WindsorResolver(container);
                var instance = resolver.GetService(typeof (IContactRepository));

                Assert.IsNotNull(instance);
            }
        }

        [Test]
        public void WindsorResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            using (var container = new WindsorContainer())
            {
                var resolver = new WindsorResolver(container);
                var instance = resolver.GetService(typeof (IContactRepository));

                Assert.IsNull(instance);
            }
        }

        [Test]
        public void WindsorResolver_Resolves_Registered_ContactRepository_Through_ContactsController_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default",
                "api/{controller}/{id}", new { id = RouteParameter.Optional });

            using (var container = new WindsorContainer())
            {
                container.Register(
                    Component.For<IContactRepository>().Instance(new InMemoryContactRepository()));

                config.DependencyResolver = new WindsorResolver(container);

                var server = new HttpServer(config);
                var client = new HttpClient(server);

				client.GetAsync("http://anything/api/contacts").ContinueWith(task =>
				{
					var response = task.Result;
					Assert.IsNotNull(response.Content);
				});
            }
        }

        [Test]
        public void WindsorResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            using (var container = new WindsorContainer())
            {

                var config = new HttpConfiguration();
                config.DependencyResolver = new WindsorResolver(container);
                var instance = config.Services.GetService(typeof (IHttpActionSelector));

                Assert.IsNotNull(instance);
            }
        }
    }
}
