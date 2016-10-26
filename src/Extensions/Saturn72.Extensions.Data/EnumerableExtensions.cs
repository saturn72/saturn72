#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;

#endregion

namespace Saturn72.Extensions
{
    public static class EnumerableExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var table = new DataTable();

            // get properties of T
            const BindingFlags binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
            const PropertyReflectionOptions options = PropertyReflectionOptions.IgnoreEnumerable | PropertyReflectionOptions.IgnoreIndexer;

            var properties = ReflectionExtensions.GetProperties<T>(binding, options).OrderBy(p=>p.Name).ToArray();

            // create table schema based on properties
            for (var i = 0; i < properties.Length; i++)
            {
                table.Columns.Add(properties[i].Name, properties[i].PropertyType);
            }

            // create table data from T instances
            var values = new object[properties.Length];
            
            foreach (var item in source)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }

            return table;
        }
    }

    [Flags]
    public enum PropertyReflectionOptions
    {
        /// <summary>
        ///     Take all.
        /// </summary>
        All = 0,

        /// <summary>
        ///     Ignores indexer properties.
        /// </summary>
        IgnoreIndexer = 1,

        /// <summary>
        ///     Ignores all other IEnumerable properties
        ///     except strings.
        /// </summary>
        IgnoreEnumerable = 2
    }

    public static class ReflectionExtensions
    {
        /// <summary>
        ///     Gets properties of T
        /// </summary>
        public static IEnumerable<PropertyInfo> GetProperties<T>(BindingFlags binding,
            PropertyReflectionOptions options = PropertyReflectionOptions.All)
        {
            var properties = typeof (T).GetProperties(binding);

            var all = (options & PropertyReflectionOptions.All) != 0;
            var ignoreIndexer = (options & PropertyReflectionOptions.IgnoreIndexer) != 0;
            var ignoreEnumerable = (options & PropertyReflectionOptions.IgnoreEnumerable) != 0;

            foreach (var property in properties)
            {
                if (!all)
                {
                    if (ignoreIndexer && IsIndexer(property))
                    {
                        continue;
                    }

                    var pType = property.PropertyType;
                    if (ignoreIndexer && pType != typeof (string) && IsEnumerable(property))
                    {
                        continue;
                    }
                }

                yield return property;
            }
        }

        /// <summary>
        ///     Check if property is indexer
        /// </summary>
        private static bool IsIndexer(PropertyInfo property)
        {
            var parameters = property.GetIndexParameters();

            if (parameters != null && parameters.Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Check if property implements IEnumerable
        /// </summary>
        private static bool IsEnumerable(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Any(x => x == typeof (IEnumerable));
        }
    }
}