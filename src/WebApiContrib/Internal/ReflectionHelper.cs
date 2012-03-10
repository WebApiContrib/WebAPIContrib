using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiContrib.Internal
{
    internal static class ReflectionHelper
    {
        public static MethodCallExpression GetMethodCall<T>(Expression<T>  action )
        {
            var call = action.Body as MethodCallExpression;

            return call;
        }

        public static string GetTypeName<T>()
        {
            return typeof (T).Name;
        }

        public static IEnumerable<Tuple<ParameterInfo, object>> GetArgumentValues(MethodCallExpression methodCall)
        {
            var parameters = methodCall.Method.GetParameters();
            if(parameters.Any())
            {
                for(int i = 0; i < parameters.Length; i++)
                {
                    var arg = methodCall.Arguments[i];

                    var ceValue = arg as ConstantExpression;

                    if (ceValue != null)
                        yield return new Tuple<ParameterInfo, object>(parameters[i], ceValue.Value);
                    else
                        yield return new Tuple<ParameterInfo, object>(parameters[i], GetExpressionValue(arg));
                }
            }
        }

        private static object GetExpressionValue(Expression expression)
        {
            var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof (object)));
            var func = lambda.Compile();
            return func();
        }
    }
}