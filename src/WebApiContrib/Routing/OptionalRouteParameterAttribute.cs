using System;

namespace WebApiContrib.Routing
{
    /// <summary>
    /// Marks a method parameter as an optional route parameter.
    /// Use this attribute in combination with the <see cref="HttpRouteAttribute"/>
    /// to declare routes that contain optional routing parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter,AllowMultiple = false, Inherited = true)]
    public class OptionalRouteParameterAttribute: Attribute
    {
    
    }
}