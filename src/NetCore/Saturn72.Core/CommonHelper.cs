using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Saturn72.Core
{
    public sealed class CommonHelper
    {
        private const string TypeNameFormat = "{0}, {1}";

        public static T Copy<T>(T source) where T : class, new()
        {
            var destination = CreateInstance<T>(typeof(T));
            Copy(source, destination);
            return destination;
        }

        public static void Copy<T>(T source, T destination)
        {
            if (source.GetType() != destination.GetType())
                throw new InvalidOperationException("Source and destination must be of the same type");

            var type = typeof(T);

            do
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var f in fields)
                    f.SetValue(destination, f.GetValue(source));
                type = type.BaseType;
            } while (type != null);
        }

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
            if (result != null)
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
                throw new ArgumentException(string.Format("The assembly {0} was not found in app domain", assemblyName),
                    assemblyName);

            return asm.GetType(typeFullName);
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
        ///     Runs expression with timeout flag
        /// </summary>
        /// <param name="expression">Expression to examined</param>
        public static bool RunTimedoutExpression(Func<bool> expression)
        {
            return RunTimedoutExpression(expression, 2000);
        }

        /// <summary>
        ///     Runs expression with timeout flag
        /// </summary>
        /// <param name="expression">Expression to examined</param>
        /// <param name="totalMiliSecTimeout">Total time out in milisecs</param>
        public static bool RunTimedoutExpression(Func<bool> expression, uint totalMiliSecTimeout)
        {
            return RunTimedoutExpression(expression, totalMiliSecTimeout, 50);
        }

        /// <summary>
        ///     Runs expression with timeout flag
        /// </summary>
        /// <param name="expression">Expression to examined</param>
        /// <param name="totalMiliSecTimeout">Total time out</param>
        /// <param name="milisecInterval">Sleep interval between expression execution</param>
        public static bool RunTimedoutExpression(Func<bool> expression, uint totalMiliSecTimeout, uint milisecInterval)
        {
            bool result;
            var startTime = DateTime.Now;

            while (!(result = expression()))
            {
                Thread.Sleep((int) milisecInterval);

                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > totalMiliSecTimeout)
                    return false;
            }

            return result;
        }

        public static string GetAssemblyLocalPath(Assembly asm)
        {
            var uri = new Uri(asm.CodeBase);
            return uri.LocalPath;
        }
    }
}