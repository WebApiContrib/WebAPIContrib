using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using WebApiContrib.IoC.Unity;
using WebApiContrib.IoC.Unity.Tests.Helpers;

namespace WebApiContrib.IoC.Unity.Tests.IoC
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void UnityResolver_Resolves_Registered_ContactRepository_Test()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IContactRepository>(new InMemoryContactRepository());

            var resolver = new UnityResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void UnityResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var container = new UnityContainer();

            var resolver = new UnityResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [Test]
        public void UnityResolver_Resolves_Registered_ContactRepository_Through_ContactsController_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default",
                "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var container = new UnityContainer();
            container.RegisterInstance<IContactRepository>(new InMemoryContactRepository());

            config.DependencyResolver = new UnityResolver(container);

            var server = new HttpServer(config);
            var client = new HttpClient(server);

        	client.GetAsync("http://anything/api/contacts").ContinueWith(task =>
        	{
        		var response = task.Result;
        		Assert.IsNotNull(response.Content);
        	});
        }

        [Test]
        public void UnityResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var container = new UnityContainer();

            var config = new HttpConfiguration();
            config.DependencyResolver = new UnityResolver(container);
            var instance = config.Services.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }
    }
}
