using System;

namespace WebApiContrib.Testing.Internal.Extensions
{
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