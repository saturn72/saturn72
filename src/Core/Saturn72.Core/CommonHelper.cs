#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using Saturn72.Core.ComponentModel;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core
{
    public class CommonHelper
    {
        private const string TypeNameFormat = "{0}, {1}";

        /// <summary>
        ///     Creates new instance of an object.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="typeName">
        ///     required object name.
        ///     <remarks>
        ///         The name foemat is [full_type_name], [assembly_name]]
        ///         <example>System, System.String</example>
        ///     </remarks>
        /// </param>
        /// <param name="args">Constructor arguments</param>
        /// <returns>new instance of </returns>
        public static T CreateInstance<T>(string typeName, params object[] args) where T : class
        {
            var type = GetTypeFromAppDomain(typeName);
            if (type == null)
                throw new ArgumentException(typeName);

            return CreateInstance<T>(type, args);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="args">Constructor arguments</param>
        /// <returns>{T}</returns>
        public static T CreateInstance<T>(Type type, params object[] args) where T : class
        {
            return Activator.CreateInstance(type, args) as T;
        }

        public static Type TryGetTypeFromAppDomain(string typeName)
        {
            var tArrLength = typeName.Split(',').Select(s => s.Trim()).ToArray().Length;

            if (tArrLength == 1)
            {
                var allAppDomainTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes());
                return
                    allAppDomainTypes.FirstOrDefault(
                        t => t.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase));
            }

            try
            {
                return GetTypeFromAppDomain(typeName);
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        ///     Fetches type by scanning app domain
        /// </summary>
        /// <param name="typeName">
        ///     required object name.
        ///     <remarks>
        ///         The name foemat is [full_type_name], [assembly_name]]
        ///         <example>System, System.String</example>
        ///     </remarks>
        /// </param>
        /// <returns>new instance of </returns>
        public static Type GetTypeFromAppDomain(string typeName)
        {
            var result = Type.GetType(typeName);
            if (result.NotNull())
                return result;

            var split = typeName.Split(',').Select(s => s.Trim()).ToArray();
            if (split.Length != 2)
                throw new ArgumentException("typeName");
            return GetTypeFromAppDomain(split[0], split[1]);
        }

        public static Type GetTypeFromAppDomain(string typeFullName, string assemblyName)
        {
            var asm = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (asm == null)
                throw new ArgumentException("The assembly {0} was not found in app domain".AsFormat(assemblyName),
                    assemblyName);

            return asm.GetType(typeFullName);
        }


        /// <summary>
        ///     Cheks if the application is Web application
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsWebApp()
        {
            return HttpRuntime.AppDomainAppId != null;
        }

        /// <summary>
        ///     Finds the trust level of the running application
        ///     (http://blogs.msdn.com/dmitryr/archive/2007/01/23/finding-out-the-current-trust-level-in-asp-net.aspx)
        /// </summary>
        /// <returns>The current trust level.</returns>
        public static AspNetHostingPermissionLevel GetTrustLevel()
        {
            //set minimum
            var trustLevel = AspNetHostingPermissionLevel.None;
            foreach (AspNetHostingPermissionLevel level in Enum.GetValues(typeof(AspNetHostingPermissionLevel)))
            {
                try
                {
                    new AspNetHostingPermission(level).Demand();
                    trustLevel = level;
                    break; //we've set the highest permission we can
                }
                catch (SecurityException)
                {
                }
            }
            return trustLevel;
        }

        public static int ToInt(string value)
        {
            int result;
            int.TryParse(value, out result);
            return result;
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            return (T) To(value, typeof(T));
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = GetCustomTypeConverter(destinationType);
                var sourceConverter = GetCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int) value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        public static TypeConverter GetCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof(List<int>))
                return new ComponentModel.GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new ComponentModel.GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new ComponentModel.GenericListTypeConverter<string>();
            if (type == typeof(Dictionary<int, int>))
                return new GenericDictionaryTypeConverter<int, int>();

            return TypeDescriptor.GetConverter(type);
        }

        public static string GetCompatibleTypeName<T>()
        {
            return GetCompatibleTypeName(typeof(T));
        }

        public static string GetCompatibleTypeName(Type type)
        {
            return string.Format(TypeNameFormat, type.FullName, type.Assembly.GetName().Name);
        }

        /// <summary>
        ///     Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email,
                "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$",
                RegexOptions.IgnoreCase);
            return result;
        }
    }
}