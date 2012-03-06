using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using Autofac;
using ContactManager.Models;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Thinktecture.Web.Http.DI;

namespace TestProject
{
    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void AutoFacResolver_Resolves_Registered_ContactRepository_Test()
        {          
            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            var resolver = new AutoFacResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void AutoFacResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var resolver = new AutoFacResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [TestMethod]
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


        [TestMethod]
        public void AutoFacResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {            
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var config = new HttpConfiguration();
            config.ServiceResolver.SetResolver(new AutoFacResolver(container));
            var instance = config.ServiceResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }
        
        [TestMethod]
        public void NinjectResolver_Resolves_Registered_ContactRepository_Test()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IContactRepository>().ToConstant(new InMemoryContactRepository());
            
            var resolver = new NinjectResolver(kernel);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void NinjectResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var kernel = new StandardKernel();

            var resolver = new NinjectResolver(kernel);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [TestMethod]
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

        [TestMethod]
        public void NinjectResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var kernel = new StandardKernel();

            var config = new HttpConfiguration();
            config.ServiceResolver.SetResolver(new NinjectResolver(kernel));
            var instance = config.ServiceResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void UnityResolver_Resolves_Registered_ContactRepository_Test()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IContactRepository>(new InMemoryContactRepository());

            var resolver = new UnityResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void UnityResolver_DoesNot_Resolve_NonRegistered_ContactRepository_Test()
        {
            var container = new UnityContainer();

            var resolver = new UnityResolver(container);
            var instance = resolver.GetService(typeof(IContactRepository));

            Assert.IsNull(instance);
        }

        [TestMethod]
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

        [TestMethod]
        public void UnityResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var container = new UnityContainer();
            
            var config = new HttpConfiguration();
            config.ServiceResolver.SetResolver(new UnityResolver(container));
            var instance = config.ServiceResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }
    }
}
