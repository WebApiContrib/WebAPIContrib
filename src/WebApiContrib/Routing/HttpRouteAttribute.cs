using System;

namespace WebApiContrib.Routing
{
    /// <summary>
    /// Declares a HTTP route entry which can be later used to build
    /// the routing table for the Web API controllers in your application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HttpRouteAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the URI template for the route.
        /// For example, api/events/{eventId}/speakers.
        /// </summary>
        public string UriTemplate { get; set; }
    }
}