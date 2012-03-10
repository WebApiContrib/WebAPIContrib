using System;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using System.Web.Routing;
using WebApiContrib.Internal.Extensions;

namespace WebApiContrib.Internal
{
    internal static class RoutingHelper
    {
        public const string CONTROLLER = "controller";

        public static RouteValueDictionary GetRouteValuesFor<TController>(Expression<Action<TController>> action) where TController : ApiController
        {
            var methodCall = ReflectionHelper.GetMethodCall(action);

            if(methodCall == null)
                throw new Exception("Action must be a method call");

            var controllerName = ReflectionHelper.GetTypeName<TController>().Replace("Controller",string.Empty);
            var arguments = ReflectionHelper.GetArgumentValues(methodCall);

            var result = new RouteValueDictionary
                             {
                                 {CONTROLLER, controllerName}, 
                             };

            arguments.ForEach(x => result.Add(x.Item1.Name, x.Item2));

            return result;
        }

        public static Uri GetUriFor(RouteValueDictionary routeValues, HttpControllerContext controllerContext)
        {

            var urlHelper = new UrlHelper(controllerContext);
            var path = urlHelper.Route("Default", routeValues);

            if (string.IsNullOrEmpty(path))
                throw new Exception(string.Format("Can't build Uri for Controller: {0}; Action {1};", routeValues[RoutingHelper.CONTROLLER]));

            return BuildUriFor(path, controllerContext);
        }

        private static Uri BuildUriFor(string path, HttpControllerContext controllerContext)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var uri = controllerContext.Request.RequestUri.Scheme + "://" +
                      controllerContext.Request.RequestUri.Authority + path;

            return new Uri(uri);
        }
    }
}