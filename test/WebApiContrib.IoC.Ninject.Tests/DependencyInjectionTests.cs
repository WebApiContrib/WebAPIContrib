using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using NUnit.Framework;
using Ninject;
using Rhino.Mocks;
using Should;
using WebApiContrib.IoC.Ninject;
using WebApiContrib.IoC.Ninject.Tests.Helpers;

namespace WebApiContrib.IoC.Ninject.Tests.IoC
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void NinjectResolver_Resolves_Registered_ContactRepository_Test()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IContactRepository>().ToConstant(new InMemoryContactRepository());

            var resolver = new NinjectResolver(kernel);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void NinjectResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var kernel = new StandardKernel();

            var resolver = new NinjectResolver(kernel);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [Test]
        public void NinjectResolver_Resolves_Registered_ContactRepository_Through_ContactsController_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default",
                "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var kernel = new StandardKernel();
            kernel.Bind<IContactRepository>().ToConstant(new InMemoryContactRepository());

            config.DependencyResolver = new NinjectResolver(kernel);

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            var response = client.GetAsync("http://anything/api/contacts").Result;

            Assert.IsNotNull(response.Content);
        }

        [Test]
        public void NinjectResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var kernel = new StandardKernel();

            var config = new HttpConfiguration();
            config.DependencyResolver = new NinjectResolver(kernel);
            var instance = config.Services.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }
    }
}
