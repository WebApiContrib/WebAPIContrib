using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace WebApiContrib.Testing
{
    //Credits to the following individuals for their posts which inspired this code
    //Pablo Cibraro: http://weblogs.asp.net/cibrax/archive/2012/10/10/unit-testing-asp-net-web-api-controllers-that-rely-on-the-urlhelper.aspx
    //Peter Provost: http://www.peterprovost.org/blog/2012/06/16/unit-testing-asp-dot-net-web-api
    public static class ApiControllerExtensions
    {
        public static void ConfigureForTesting(this ApiController controller, HttpMethod method, string uri, 
                                               string routeName = null, IHttpRoute route = null)
        {
            var request = new HttpRequestMessage(method, uri);
            ConfigureForTesting(controller, request, routeName, route);
        }

        public static void ConfigureForTesting(this ApiController controller, HttpRequestMessage request, string routeName = null, IHttpRoute route = null)
        {
            var config = new HttpConfiguration();
            controller.Configuration = config;
            if (routeName != null && route != null)
                config.Routes.Add(routeName, route);
            else
                route = config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });

            var controllerTypeName = controller.GetType().Name;
            var controllerName = controllerTypeName.Substring(0, controllerTypeName.IndexOf("Controller")).ToLower();
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", controllerName } });
            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.ControllerContext.ControllerDescriptor = new HttpControllerDescriptor(config, controllerName, controller.GetType());
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
        }
    }
}
