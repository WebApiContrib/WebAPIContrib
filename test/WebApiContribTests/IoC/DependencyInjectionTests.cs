using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Autofac;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ninject;
using WebApiContrib.IoC.AutoFac;
using WebApiContrib.IoC.Ninject;
using WebApiContrib.IoC.Unity;

namespace WebApiContribTests.IoC
{
    [TestFixture]
    public class DependencyInjectionTests
    {
        [Test]
        public void AutoFacResolver_Resolves_Registered_ContactRepository_Test()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            var resolver = new AutoFacResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void AutoFacResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var resolver = new AutoFacResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [Test]
        public void AutoFacResolver_Resolves_Registered_ContactRepository_ThroughHost_Test()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("default",
                "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            config.ServiceResolver.SetResolver(new AutoFacResolver(container));

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            var response = client.GetAsync("http://anything/api/contacts").Result;

            Assert.IsNotNull(response.Content);
        }


        [Test]
        public void AutoFacResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var config = new HttpConfiguration();
            config.ServiceResolver.SetResolver(new AutoFacResolver(container));
            var instance = config.ServiceResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }

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

            config.ServiceResolver.SetResolver(new NinjectResolver(kernel));

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
            config.ServiceResolver.SetResolver(new NinjectResolver(kernel));
            var instance = config.ServiceResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }

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

            config.ServiceResolver.SetResolver(new UnityResolver(container));

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            var response = client.GetAsync("http://anything/api/contacts").Result;

            Assert.IsNotNull(response.Content);
        }

        [Test]
        public void UnityResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var container = new UnityContainer();

            var config = new HttpConfiguration();
            config.ServiceResolver.SetResolver(new UnityResolver(container));
            var instance = config.ServiceResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }
    }

    public class InMemoryContactRepository : IContactRepository { }

    public interface IContactRepository { }
}