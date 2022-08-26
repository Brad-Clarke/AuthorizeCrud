using System;
using System.Collections.Generic;

namespace ModelAuthorization.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static Type GetGenericType<T>(this IEnumerable<T> enumerable)
        {
            return typeof(T);
        }
    }
}
