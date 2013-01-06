// Modified from http://mvccontrib.codeplex.com/ to work with Web API
// Copyright 2007-2010 Eric Hexter, Jeffrey Palermo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using HttpVerbs = System.Web.Mvc.HttpVerbs;
using UrlParameter = System.Web.Mvc.UrlParameter;
using System.Web.Routing;
using MvcContrib.TestHelper;

namespace WebApiContrib.Testing
{
    /// <summary>
    /// Allows testing that a url routes to the appropriate controller action
    /// </summary>
    public static class RouteTestingExtensions
    {
        private static readonly Dictionary<HttpVerbs, Type> HttpMethodLookup = new Dictionary<HttpVerbs, Type>
            {
                {HttpVerbs.Get, typeof(HttpGetAttribute)},
                {HttpVerbs.Post, typeof(HttpPostAttribute)},
                {HttpVerbs.Delete, typeof(HttpDeleteAttribute)},
                {HttpVerbs.Put, typeof(HttpPutAttribute)}
            };

        /// <summary>
        /// Asserts that the specified url routes to the expected controller and action
        /// <example>"~/api/sample/5".ShouldRouteTo&lt;SampleController&gt;(x => x.Get(5))</example>
        /// </summary>
        /// <typeparam name="TController">The expected controller that the url should map to</typeparam>
        /// <param name="relativeUrl">The url to verify</param>
        /// <param name="action">The expected action that the url should map to</param>
        /// <param name="httpMethod">The HTTP method, e.g. Get, Post, Put, Delete</param>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, object>> action, HttpVerbs httpMethod = HttpVerbs.Get) where TController : ApiController
        {
            return relativeUrl.Route(httpMethod).ShouldMapTo(action, httpMethod);
        }

        private static RouteData ShouldMapTo<TController>(this RouteData routeData) where TController : ApiController
        {
            //strip out the word 'Controller' from the type
            string expected = typeof(TController).Name;
            if (expected.EndsWith("Controller"))
            {
                expected = expected.Substring(0, expected.LastIndexOf("Controller", StringComparison.Ordinal));
            }

            //get the key (case insensitive)
            string actual = routeData.Values.GetValue("controller").ToString();

            actual.AssertSameStringAs(expected);
            return routeData;
        }

        private static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, object>> action, HttpVerbs httpMethod) where TController : ApiController
        {
            routeData.ShouldNotBeNull("The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = (routeData.Values.GetValue("action") ?? httpMethod.ToString()).ToString();
            string expectedAction = methodCall.Method.ActionName();
            actualAction.AssertSameStringAs(expectedAction);

            // If convention is not being used, verify that the correct httpMethod attribute is present
            if (string.Compare(httpMethod.ToString(), actualAction, StringComparison.OrdinalIgnoreCase) != 0)
            {
                methodCall.Method.HasAttribute(HttpMethodLookup[httpMethod]).ShouldBe(true);
            }

            //check parameters
            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                ParameterInfo param = methodCall.Method.GetParameters()[i];
                bool isReferenceType = !param.ParameterType.IsValueType;
                bool isNullable = isReferenceType ||
                                  (param.ParameterType.UnderlyingSystemType.IsGenericType && param.ParameterType.UnderlyingSystemType.GetGenericTypeDefinition() == typeof(Nullable<>));

                string controllerParameterName = param.Name;
                bool routeDataContainsValueForParameterName = routeData.Values.ContainsKey(controllerParameterName);
                object actualValue = routeData.Values.GetValue(controllerParameterName);
                object expectedValue = null;
                Expression expressionToEvaluate = methodCall.Arguments[i];

                // If the parameter is nullable and the expression is a Convert UnaryExpression, 
                // we actually want to test against the value of the expression's operand.
                if (expressionToEvaluate.NodeType == ExpressionType.Convert
                    && expressionToEvaluate is UnaryExpression)
                {
                    expressionToEvaluate = ((UnaryExpression)expressionToEvaluate).Operand;
                }

                switch (expressionToEvaluate.NodeType)
                {
                    case ExpressionType.Constant:
                        expectedValue = ((ConstantExpression)expressionToEvaluate).Value;
                        break;

                    case ExpressionType.New:
                    case ExpressionType.MemberAccess:
                        expectedValue = Expression.Lambda(expressionToEvaluate).Compile().DynamicInvoke();
                        break;
                }

                if (isNullable && (string)actualValue == String.Empty && expectedValue == null)
                {
                    // The parameter is nullable so an expected value of '' is equivalent to null;
                    continue;
                }

                // The parameter is not used in the route (e.g. a parameter from the query string)
                if (!isNullable && !routeDataContainsValueForParameterName)
                {
                    var defaultValue = param.ParameterType.GetDefault();
                    actualValue = defaultValue != null ? defaultValue.ToString() : string.Empty;
                }

                // this is only sufficient while System.Web.Mvc.UrlParameter has only a single value.
                if (actualValue == UrlParameter.Optional ||
                    (actualValue != null && actualValue.ToString().Equals("System.Web.Mvc.UrlParameter")))
                {
                    actualValue = null;
                }

                if (expectedValue is DateTime)
                {
                    actualValue = Convert.ToDateTime(actualValue);
                }
                else
                {
                    expectedValue = (expectedValue == null ? expectedValue : expectedValue.ToString());
                }

                string errorMsgFmt = "Value for parameter '{0}' did not match: expected '{1}' but was '{2}'";
                if (routeDataContainsValueForParameterName)
                {
                    errorMsgFmt += ".";
                }
                else
                {
                    errorMsgFmt += "; no value found in the route context action parameter named '{0}' - does your matching route contain a token called '{0}'?";
                }

                actualValue.ShouldEqual(expectedValue, String.Format(errorMsgFmt, controllerParameterName, expectedValue, actualValue));
            }

            return routeData;
        }
    }

    internal static class MemberInfoExtensions
    {
        public static IEnumerable<T> GetAttribute<T>(this MemberInfo member)
            where T : class
        {
            return GetAttribute(member, typeof(T)).OfType<T>();
        }

        public static IEnumerable<object> GetAttribute(this MemberInfo member, Type attributeType)
        {
            return member.GetCustomAttributes(attributeType, true);
        }

        public static bool HasAttribute<T>(this MemberInfo member)
            where T : class
        {
            return HasAttribute(member, typeof(T));
        }

        public static bool HasAttribute(this MemberInfo member, Type attributeType)
        {
            return member.GetAttribute(attributeType).Any();
        }
    }

    internal static class TypeExtensions
    {
        public static Type GetTypeFromNullable(this Type type)
        {
            if (!type.IsGenericType)
                return type;

            if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return type;

            return type.GetGenericArguments()[0];
        }

        public static object GetDefault(this Type type)
        {
            return typeof(TypeExtensions).GetMethod("GetDefaultGeneric").MakeGenericMethod(type).Invoke(null, null);
        }

        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }
    }
}