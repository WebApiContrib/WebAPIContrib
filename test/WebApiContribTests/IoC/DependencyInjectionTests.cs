using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Autofac;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Ninject;
using Rhino.Mocks;
using Should;
using StructureMap;
using WebApiContrib.IoC.Autofac;
using WebApiContrib.IoC.CastleWindsor;
using WebApiContrib.IoC.Ninject;
using WebApiContrib.IoC.StructureMap;
using WebApiContrib.IoC.Unity;
using WebApiContribTests.Helpers;

namespace WebApiContribTests.IoC
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
            config.Routes.MapHttpRoute("default",
                "api/{controller}/{id}", new { id = RouteParameter.Optional });

            var builder = new ContainerBuilder();
            builder.RegisterType<InMemoryContactRepository>().As<IContactRepository>();
            var container = builder.Build();

            config.DependencyResolver = new AutofacResolver(container);

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            var response = client.GetAsync("http://anything/api/contacts").Result;

            Assert.IsNotNull(response.Content);
        }


        [Test]
        public void AutofacResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var config = new HttpConfiguration();
            config.DependencyResolver = new AutofacResolver(container);
            var instance = config.DependencyResolver.GetService(typeof(IHttpActionSelector));

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
            var instance = config.DependencyResolver.GetService(typeof(IHttpActionSelector));

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

            config.DependencyResolver = new UnityResolver(container);

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
            config.DependencyResolver = new UnityResolver(container);
            var instance = config.DependencyResolver.GetService(typeof(IHttpActionSelector));

            Assert.IsNotNull(instance);
        }

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

                var response = client.GetAsync("http://anything/api/contacts").Result;

                Assert.IsNotNull(response.Content);
            }
        }

        [Test]
        public void WindsorResolver_In_HttpConfig_DoesNot_Resolve_PipelineType_But_Fallback_To_DefaultResolver_Test()
        {
            using (var container = new WindsorContainer())
            {

                var config = new HttpConfiguration();
                config.DependencyResolver = new WindsorResolver(container);
                var instance = config.DependencyResolver.GetService(typeof (IHttpActionSelector));

                Assert.IsNotNull(instance);
            }
        }

        [Test]
        public void StructureMapResolver_should_be_returned_for_IHttpControllerActivator_WithDefaultConventions()
        {
            // When building up a SM container, this is the only thing that doesn't get
            // resolved "nicely" when using default conventions.
            var container = new Container(x => x.Scan(s =>
            {
                s.TheCallingAssembly();
                s.WithDefaultConventions();
            }));
            var config = new HttpConfiguration();
            var resolver = new StructureMapResolver(container);

            config.DependencyResolver = resolver;

            var actualActivator = config.DependencyResolver.GetService(typeof (IHttpControllerActivator));

            actualActivator.ShouldBeSameAs(resolver);
        }

        [Test]
        public void StructureMapResolver_should_return_both_instaces_of_IContactRepository()
        {
            var container = new Container(x => x.Scan(s =>
            {
                s.TheCallingAssembly();
                s.AddAllTypesOf<IContactRepository>().NameBy(t => t.Name);
            }));
            var config = new HttpConfiguration();
            var resolver = new StructureMapResolver(container);

            config.DependencyResolver = resolver;

            var repositories = config.DependencyResolver.GetServices(typeof(IContactRepository));
            repositories.Count().ShouldEqual(2);
        }

        [Test]
        public void StructureMapResolver_should_return_an_empty_collection_if_type_isnt_found()
        {
            var config = new HttpConfiguration();
            var resolver = new StructureMapResolver(new Container());

            config.DependencyResolver = resolver;

            var repositories = config.DependencyResolver.GetServices(typeof(IContactRepository));
            repositories.Count().ShouldEqual(0);
        }

        [Test]
        public void StructureMapResolver_Create_should_delegate_to_GetInstance()
        {
            var container = MockRepository.GenerateMock<StructureMap.IContainer>();
            var resolver = new StructureMapResolver(container);
            var controllerType = typeof (ContactsController);

            resolver.Create(new HttpRequestMessage(), new HttpControllerDescriptor(), controllerType);

            container.AssertWasCalled(x => x.GetInstance(controllerType));
        }
    }
}
