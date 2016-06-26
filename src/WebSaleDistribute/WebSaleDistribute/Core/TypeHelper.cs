using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Routing;

namespace WebSaleDistribute.Core
{
    internal static class TypeHelper
    {
        public static RouteValueDictionary ToRouteValueDictionary(this object value)
        {
            var routeValueDictionary = new RouteValueDictionary();

            if (value == null) return routeValueDictionary;

            foreach (var property in value.GetType().GetProperties())
                routeValueDictionary.Add(property.Name, property.GetValue(value));
            return routeValueDictionary;
        }

        public static IDictionary<string, object> ToDictionary(this object value)
        {
            return value.ToRouteValueDictionary();
        }

        public static void AddAnonymousObjectToDictionary(this IDictionary<string, object> dictionary, object value)
        {
            foreach (KeyValuePair<string, object> keyValuePair in value.ToDictionary())
                dictionary.Add(keyValuePair);
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type == (Type)null)
                throw new ArgumentNullException(nameof(type));

            if (!Attribute.IsDefined((MemberInfo)type, typeof(CompilerGeneratedAttribute), false)
                || !type.IsGenericType
                || !type.Name.Contains("AnonymousType")
                || !type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase)
                && !type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                return false;

            int num = (int)type.Attributes;
            return true;
        }
    }
}