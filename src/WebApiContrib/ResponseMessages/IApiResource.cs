using System;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebApiContrib.Internal;

namespace WebApiContrib.ResponseMessages
{
    public interface IApiResource
    {
        void SetLocation(ResourceLocation location);
    }

    public class ResourceLocation
    {
        private readonly HttpControllerContext _controllerContext;
        public Uri Location { get; private set; }

        public ResourceLocation(HttpControllerContext controllerContext)
        {
            _controllerContext = controllerContext;
        }

        public void Set(Uri location)
        {
            Location = location;
        }

        /// <summary>
        /// Sets the Location Header of the Resource to a URI that would be routed to the given controller method. 
        /// Right now this only works using the "Default" route, because HttpRouteCollection provides no way to programatically discover route names and there is no method in UrlHelper to get an URI without
        /// specifying witch route name to use.
        /// </summary>
        /// <typeparam name="TController">Controller type</typeparam>
        /// <param name="action">Method to be called in the controller</param>
        public void SetIn<TController>(Expression<Action<TController>> action) where TController : ApiController
        {
            var routeValues = RoutingHelper.GetRouteValuesFor(action);

            var uri = RoutingHelper.GetUriFor(routeValues, _controllerContext);

            Location = uri;

        }
    }
}

