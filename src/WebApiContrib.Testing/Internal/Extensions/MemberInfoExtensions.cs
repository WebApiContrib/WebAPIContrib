using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApiContrib.Testing.Internal.Extensions
{
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
}