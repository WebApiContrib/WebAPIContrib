using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Http.WebHost.Routing;
using System.Web.Routing;

namespace WebApiContrib.Routing
{
    /// <summary>
    /// Route table builder that uses the <see cref="HttpRouteAttribute"/> instances found on the controllers
    /// in the web application.
    /// </summary>
    public class HttpRouteTableBuilder
    {
        /// <summary>
        /// Builds a route table based on the Http route 
        /// attributes found on the controllers in the current web application.
        /// </summary>
        /// <param name="routes">Route collection to append the routes to</param>
        public static void BuildTable(RouteCollection routes)
        {
            var assembly = Assembly.GetExecutingAssembly();
            BuildTable(routes,assembly);
        }

        /// <summary>
        /// Builds a route table based on the Http route 
        /// attributes found on the controllers in the current web application.
        /// </summary>
        /// <param name="routes">Route collection to append the routes to</param>
        /// <param name="assembly">Assembly to scan for routes</param>
        public static void BuildTable(RouteCollection routes,Assembly assembly)
        {
            var controllerTypes = assembly.GetExportedTypes()
                .Where(type => typeof(ApiController).IsAssignableFrom(type));

            // Find all the controller types and extract the HTTP route attributes from 
            // the public methods that can be found on the controller.
            foreach (var controllerType in controllerTypes)
            {
                BuildControllerRoutes(routes, controllerType);

                var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    BuildControllerMethodRoutes(routes, controllerType, method);
                }
            }
        }

        /// <summary>
        /// Builds the contents for the route table based on the attributes
        /// found on a specific controller type.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="controllerType"></param>
        private static void BuildControllerRoutes(RouteCollection routes, Type controllerType)
        {
            HttpRouteAttribute[] attributes = (HttpRouteAttribute[])controllerType.GetCustomAttributes(
                typeof(HttpRouteAttribute), true);

            string controller = controllerType.Name;

            // Translate the somewhat weird controller name into one the routing system
            // understands, by removing the Controller part from the name.
            if (controller.EndsWith("Controller", StringComparison.Ordinal))
            {
                controller = controller.Substring(0, controller.IndexOf("Controller"));
            }

            foreach (var attribute in attributes)
            {
                RouteValueDictionary routeValuesDictionary = new RouteValueDictionary();
                routeValuesDictionary.Add("controller", controller);

                // Create the route and attach the default route handler to it.
                HttpWebRoute route = new HttpWebRoute(attribute.UriTemplate, routeValuesDictionary,
                    new RouteValueDictionary(), new RouteValueDictionary(), HttpControllerRouteHandler.Instance);

                routes.Add(Guid.NewGuid().ToString(), route);
            }
        }

        /// <summary>
        /// Builds the contents for the route table based on the attributes
        /// found on the method of a specific controller.
        /// </summary>
        /// <param name="routes"></param>
        /// <param name="controllerType"></param>
        /// <param name="method"></param>
        private static void BuildControllerMethodRoutes(RouteCollection routes, Type controllerType, MethodInfo method)
        {
            // Grab the http route attributes from the current method.
            HttpRouteAttribute[] attributes = (HttpRouteAttribute[])method.GetCustomAttributes(
                typeof(HttpRouteAttribute), true);

            if (attributes.Length != 0)
            {
                // Automatically grab the controller name and action name
                // from the method and controller type.
                string action = method.Name;
                string controller = controllerType.Name;

                // Translate the somewhat weird controller name into one the routing system
                // understands, by removing the Controller part from the name.
                if (controller.EndsWith("Controller", StringComparison.Ordinal))
                {
                    controller = controller.Substring(0, controller.IndexOf("Controller"));
                }

                // Generate a route for every HTTP route attribute found on the method
                foreach (var attribute in attributes)
                {
                    var routeValueDictionary = new RouteValueDictionary();

                    routeValueDictionary.Add("controller", controller);
                    routeValueDictionary.Add("action", action);

                    ResolveOptionalRouteParameters(attribute.UriTemplate, method, routeValueDictionary);

                    // Create the route and attach the default route handler to it.
                    HttpWebRoute route = new HttpWebRoute(attribute.UriTemplate, routeValueDictionary,
                        new RouteValueDictionary(), new RouteValueDictionary(), HttpControllerRouteHandler.Instance);

                    routes.Add(Guid.NewGuid().ToString(), route);
                }
            }
        }

        /// <summary>
        /// Resolves any route parameters that have been marked as optional
        /// </summary>
        /// <param name="uriTemplate"></param>
        /// <param name="method"></param>
        /// <param name="routeValueDictionary"></param>
        private static void ResolveOptionalRouteParameters(string uriTemplate, MethodInfo method, RouteValueDictionary routeValueDictionary)
        {
            Regex pattern = new Regex(@"{(\S+)}");
            var methodParameters = method.GetParameters();

            foreach (Match match in pattern.Matches(uriTemplate))
            {
                string parameterName = match.Groups[1].Value;
                var parameter = methodParameters.FirstOrDefault(param => param.Name == parameterName);

                // Mark the route parameter as optional when there's a method parameter for it
                // and that method parameter is marked with [OptionalRouteParameter]
                if (parameter != null && parameter.GetCustomAttributes(typeof(OptionalRouteParameterAttribute), true).Length != 0)
                {
                    routeValueDictionary.Add(parameterName, RouteParameter.Optional);
                }
            }
        }
    }

}