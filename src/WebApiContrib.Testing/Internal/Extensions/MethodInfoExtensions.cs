// Modified from http://mvccontrib.codeplex.com
// Copyright 2007-2010 Eric Hexter, Jeffrey Palermo

using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace WebApiContrib.Testing.Internal.Extensions
{
    internal static class MethodInfoExtensions
    {
        /// <summary>
        /// Returns the name of the action specified in the ActionNameAttribute or the name of the method if no attribute is present.
        /// </summary>
        /// <param name="method"></param>
        public static string ActionName(this MethodInfo method)
        {
            if (method.HasAttribute<ActionNameAttribute>())
                return method.GetAttribute<ActionNameAttribute>().First().Name;

            return method.Name;
        }
    }
}