using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Autofac;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using WebApiContrib.IoC.Autofac;
using WebApiContrib.IoC.Autofac.Tests.Helpers;

namespace WebApiContrib.IoC.Autofac.Tests
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void AutofacResolver_Resolves_Registered_ContactRepository_Test()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            var resolver = new AutofacResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void AutofacResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var resolver = new AutofacResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [Test]
        public void AutofacResolver_Resolves_Registered_ContactRepository_ThroughHost_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            config.DependencyResolver = new AutofacResolver(container);

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            client.GetAsync("http://anything/api/contacts").ContinueWith(task =>
            {
                var response = task.Result;
                Assert.IsNotNull(response.Content);
            });
        }

        [Test]
        public void AutofacResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var config = new HttpConfiguration();
            config.DependencyResolver = new AutofacResolver(container);
            var instance = config.Services.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void AutofacResolver_DoesNot_Resolve_NonRegistered_ContactRepositories_Test()
        {
        	var builder = new ContainerBuilder();
        	var container = builder.Build();

            var config = new HttpConfiguration();
            config.DependencyResolver = new AutofacResolver(container);
            var repositories = config.DependencyResolver.GetServices(typeof(IContactRepository));

            repositories.Count().ShouldEqual(0);
        }

        [Test]
        public void AutofacResolver_Resolves_Registered_Both_Instaces_Of_IContactRepository()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            builder.RegisterType<FileContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            var config = new HttpConfiguration();
            var resolver = new AutofacResolver(container);

            config.DependencyResolver = resolver;
            var repositories = config.DependencyResolver.GetServices(typeof(IContactRepository));

            repositories.Count().ShouldEqual(2);
        }
    }
}
