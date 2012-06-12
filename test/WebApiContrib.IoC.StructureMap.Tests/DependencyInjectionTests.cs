using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using NUnit.Framework;
using Rhino.Mocks;
using Should;
using StructureMap;
using WebApiContrib.IoC.StructureMap;
using WebApiContrib.IoC.StructureMap.Tests.Helpers;

namespace WebApiContrib.IoC.StructureMap.Tests.IoC
{
    [TestFixture]
    public class DependencyInjectionTests
    {
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

            var actualActivator = config.Services.GetService(typeof (IHttpControllerActivator));

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
            var container = MockRepository.GenerateMock<IContainer>();
            var resolver = new StructureMapResolver(container);
            var controllerType = typeof (ContactsController);

            resolver.Create(new HttpRequestMessage(), new HttpControllerDescriptor(), controllerType);

            container.AssertWasCalled(x => x.GetInstance(controllerType));
        }
    }
}
