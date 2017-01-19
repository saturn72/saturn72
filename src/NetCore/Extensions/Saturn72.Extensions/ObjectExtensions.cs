using System;

namespace Saturn72.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsDefaultOrNull(this object obj)
        {
            return IsDefault(obj) || IsNull(obj);
        }

        public static bool IsDefaultOrNull(this DateTime? obj)
        {
            return obj == default(DateTime) || obj ==null;
        }

        public static bool IsNull(this object o)
        {
            return o == null;
        }

        public static bool NotNull(this object o)
        {
            return !IsNull(o);
        }

        public static bool IsDefault<T>(this T o)
        {
            var d = default(T);
            return (o == null && d == null)
                   || o.Equals(d);
        }

        public static bool NotDefault<T>(this T o)
        {
            return !IsDefault(o);
        }
    }
}