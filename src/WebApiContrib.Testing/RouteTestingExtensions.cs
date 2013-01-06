// Modified from http://mvccontrib.codeplex.com to work with Web API
// Copyright 2007-2010 Eric Hexter, Jeffrey Palermo

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Rhino.Mocks;
using WebApiContrib.Testing.Internal.Extensions;

namespace WebApiContrib.Testing
{
    /// <summary>
    /// Allows testing that a url is routed to the appropriate controller action
    /// </summary>
    public static class RouteTestingExtensions
    {
        private static readonly Dictionary<string, Type> httpMethodLookup = new Dictionary<string, Type>
            {
                {"GET", typeof(HttpGetAttribute)},
                {"POST", typeof(HttpPostAttribute)},
                {"DELETE", typeof(HttpDeleteAttribute)},
                {"PUT", typeof(HttpPutAttribute)},
                {"HEAD", typeof(HttpHeadAttribute)},
                {"PATCH", typeof(HttpPatchAttribute)},
                {"OPTIONS", typeof(HttpOptionsAttribute)},
            };

        /// <summary>
        /// Asserts that the specified url routes to the expected controller and action
        /// <example>"~/api/sample/5".ShouldRouteTo&lt;SampleController&gt;(x => x.Get(5))</example>
        /// </summary>
        /// <typeparam name="TController">The expected controller that the url should map to</typeparam>
        /// <param name="relativeUrl">The url to verify</param>
        /// <param name="action">The expected action that the url should map to</param>
        /// <param name="httpMethod">The HTTP method, e.g. Get, Post, Put, Delete</param>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, object>> action, string httpMethod = "GET") where TController : ApiController
        {
            return relativeUrl.Route(httpMethod).ShouldMapTo(action, httpMethod);
        }

        private static RouteData ShouldMapTo<TController>(this RouteData routeData) where TController : ApiController
        {
            //strip out the word 'Controller' from the type
            string expectedController = typeof(TController).Name;
            if (expectedController.EndsWith("Controller"))
            {
                expectedController = expectedController.Substring(0, expectedController.LastIndexOf("Controller", StringComparison.Ordinal));
            }

            //get the key (case insensitive)
            string actualController = routeData.Values.GetValue("controller").ToString();

            Assert.AreSameString(actualController, expectedController);
            return routeData;
        }

        private static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, object>> action, string httpMethod) where TController : ApiController
        {
            Assert.IsNotNull(routeData, "The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = (routeData.Values.GetValue("action") ?? httpMethod).ToString();
            string expectedAction = methodCall.Method.ActionName();
            Assert.AreSameString(expectedAction, actualAction);

            // If convention is not being used, verify that the correct httpMethod attribute is present
            if (string.Compare(httpMethod, actualAction, StringComparison.OrdinalIgnoreCase) != 0)
            {
                bool hasHttpMethodAttribute = methodCall.Method.HasAttribute(httpMethodLookup[httpMethod]);
                Assert.IsTrue(hasHttpMethodAttribute);
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
                if (actualValue == RouteParameter.Optional ||
                    (actualValue != null && actualValue.ToString().Equals(typeof(RouteParameter).FullName)))
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

                Assert.AreEqual(expectedValue, actualValue, String.Format(errorMsgFmt, controllerParameterName, expectedValue, actualValue));
            }

            return routeData;
        }

        /// <summary>
        /// Gets a value from the <see cref="RouteValueDictionary" /> by key. Does a culture and case insensitive search on the keys.
        /// </summary>
        /// <param name="routeValues">The route values</param>
        /// <param name="key">The key from which to retrieve the value</param>
        /// <returns>Returns the requested route value or null the key is not present</returns>
        public static object GetValue(this RouteValueDictionary routeValues, string key)
        {
            foreach(var routeValueKey in routeValues.Keys)
            {
                if(!string.Equals(routeValueKey, key, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                if(routeValues[routeValueKey] == null)
                    return null;

                return routeValues[routeValueKey].ToString();
            }

            return null;
        }

        /// <summary>
        /// Find the route for a URL and an Http Method
        /// </summary>
        /// <param name="url">The URL to route</param>
        /// <param name="httpMethod">The HTTP method (e.g. GET, PUT, etc)</param>
        /// <returns>The route or null if not route matched</returns>
        public static RouteData Route(this string url, string httpMethod = "GET")
        {
            var mockRequest = MockRepository.GeneratePartialMock<HttpRequestBase>();
            mockRequest
                .Expect(x => x.AppRelativeCurrentExecutionFilePath)
                .Return(url)
                .Repeat.Any();
            mockRequest
                .Expect(x => x.PathInfo)
                .Return(string.Empty)
                .Repeat.Any();
            mockRequest
                .Expect(x => x.HttpMethod)
                .Return(httpMethod)
                .Repeat.Any();

            var mockContext = MockRepository.GeneratePartialMock<HttpContextBase>();
            mockContext
                .Expect(x => x.Request)
                .Return(mockRequest)
                .Repeat.Any();

            return RouteTable.Routes.GetRouteData(mockContext);
        }
    }
}