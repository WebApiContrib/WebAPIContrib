using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using NUnit.Framework;
using WebApiContrib.Testing;
using Should;

namespace WebApiContribTests.Testing
{
    [TestFixture]
    public class ApiControllerExtensionsTests
    {
        private DummyController controller;
        private HttpRequestMessage request;

        [SetUp]
        public void TestFixtureSetUp()
        {
            request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");
            controller = new DummyController();
            controller.ConfigureForTesting(request);
        }

        [Test]
        public void ShouldCreateControllerConfigurationWhenConfigureIsInvoked()
        {
            controller.Configuration.ShouldNotBeNull();
        }

        [Test]
        public void ShouldAddRouteIfProvidedWhenConfigureIsInvoked()
        {
            var route = new HttpRoute("testroute");
            controller = new DummyController();
            controller.ConfigureForTesting(request, "testroute", route);
            controller.Configuration.Routes.ShouldContain(route);
        }

        [Test]
        public void ShouldSetDefaultRouteIfNoneIsProvidedWhenConfigureIsInvoked()
        {
            var route = controller.Configuration.Routes["DefaultApi"];
            route.ShouldNotBeNull();
        }

        [Test]
        public void ShouldCreateRequestWhenConfigureIsInvokedPassingUriAndMethod()
        {
            controller.ConfigureForTesting(HttpMethod.Get, "http://localhost/test");
            controller.Request.Method.ShouldEqual(HttpMethod.Get);
            controller.Request.RequestUri.AbsoluteUri.ShouldEqual("http://localhost/test");
        }

        [Test]
        public void ShouldSetRouteDataWithControllerInformationWhenConfigureIsInvoked()
        {
            controller = new DummyController();
            var route = new HttpRoute("testroute");
            controller.ConfigureForTesting(new HttpRequestMessage(), "testroute", route);
            var addedRoute = controller.Configuration.Routes["testroute"];
            addedRoute.ShouldEqual(route);
        }

        [Test]
        public void ShouldCreateControllerContextWhenConfigureIsInvoked()
        {
            var context = controller.ControllerContext;
            context.ShouldNotBeNull();
            context.Configuration.ShouldEqual(controller.Configuration);
            context.RouteData.Route.ShouldEqual(controller.Configuration.Routes["DefaultApi"]);
            context.Request.ShouldEqual(request);
        }

        [Test]
        public void ShouldSetControllerDescriptorWhenConfigureIsInvoked()
        {
            var descriptor = controller.ControllerContext.ControllerDescriptor;
            descriptor.ShouldNotBeNull();
            descriptor.Configuration.ShouldEqual(controller.Configuration);
            descriptor.ControllerName.ShouldEqual("dummy");
            descriptor.ControllerType.ShouldEqual(typeof (DummyController));
        }

        [Test]
        public void ShouldSetControllerRequestToRequestWhenConfigureIsInvoked()
        {
            controller.Request.ShouldEqual(request);
        }

        [Test]
        public void ShouldSetHttpConfigurationKeyToConfigurationWhenConfigureIsInvoked()
        {
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey].ShouldEqual(controller.Configuration);
        }

        [Test]
        public void ShouldSetHttpRouteDataKeyToRouteDataWhenConfigureIsInvoked()
        {
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey].ShouldEqual(controller.ControllerContext.RouteData);
        }

    }
}
