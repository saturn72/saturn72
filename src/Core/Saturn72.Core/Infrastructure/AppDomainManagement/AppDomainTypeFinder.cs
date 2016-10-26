#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

#endregion

namespace Saturn72.Core.Infrastructure.AppDomainManagement
{
    public class AppDomainTypeFinder : ITypeFinder
    {
        #region Fields

        private bool _loadAppDomainAssemblies = true;

        private IList<string> assemblyNames = new List<string>();
        private string assemblyRestrictToLoadingPattern = ".*";

        private string assemblySkipLoadingPattern =
            "^System|^mscorlib|^vshost|^Microsoft|^Autofac|^AutoMapper|^Castle|^EntityFramework|^FluentValidation|^log4net|^Newtonsoft|^nunit|^xunit|^Recaptcha|^Telerik|^MongoDB|^Sqlite|^NLog|^RestSharp|^Antlr|^Owin|^WebGrease";

        private bool ignoreReflectionErrors = true;

        #endregion Fields

        #region Properties

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain WatchedAppDomain
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>
        ///     Gets or sets wether Nop should iterate assemblies in the app domain when loading Nop types. Loading patterns
        ///     are applied when loading these assemblies.
        /// </summary>
        public bool LoadAppDomainAssemblies
        {
            get { return _loadAppDomainAssemblies; }
            set { _loadAppDomainAssemblies = value; }
        }

        /// <summary>Gets or sets assemblies loaded a startup in addition to those loaded in the AppDomain.</summary>
        public IList<string> AssemblyNames
        {
            get { return assemblyNames; }
            set { assemblyNames = value; }
        }

        /// <summary>Gets the pattern for dlls that we know don'type need to be investigated.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return assemblySkipLoadingPattern; }
            set { assemblySkipLoadingPattern = value; }
        }

        /// <summary>
        ///     Gets or sets the pattern for dll that will be investigated. For ease of use this defaults to match all but to
        ///     increase performance you might want to configure a pattern that includes assemblies and your own.
        /// </summary>
        /// <remarks>
        ///     If you change this so that Nop assemblies arn'type investigated (e.g. by not including something like
        ///     "^Nop|..." you may break core functionality.
        /// </remarks>
        public string AssemblyRestrictToLoadingPattern
        {
            get { return assemblyRestrictToLoadingPattern; }
            set { assemblyRestrictToLoadingPattern = value; }
        }

        #endregion Properties

        #region Methods

        public IEnumerable<Type> FindClassesOfType<TType>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof (TType), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof (T), assemblies, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true)
        {
            Func<Type, IEnumerable<Type>> func = t => 
                assignTypeFrom.IsAssignableFrom(t) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(t, assignTypeFrom))

                ? ReturnTypeEnumerableWhenOnlyConcreteClassesRequired(t, onlyConcreteClasses)
                : new Type[] {};

            return SearchAssembliesBySearchCriteria(assemblies, func);
        }

        public IEnumerable<MethodInfo> FindMethodsOfReturnType<TReturnType>(bool onlyConcreteClasses = true)
        {
            return FindMethodsOfReturnType(typeof (TReturnType), onlyConcreteClasses);
        }

        public IEnumerable<MethodInfo> FindMethodsOfReturnType(Type returnType, bool onlyConcreteClasses = true)
        {
            return FindMethodsOfReturnType(returnType, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<MethodInfo> FindMethodsOfReturnType(Type returnType, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true)
        {
            //return SearchMethodsMatchesSearchCriteria(assemblies, m => m.ReturnType.IsAssignableFrom(returnType));
            Func<MethodInfo, bool> func = mi =>
                returnType.IsAssignableFrom(mi.ReturnType) &&
                ReturnTypeEnumerableWhenOnlyConcreteClassesRequired(mi.ReflectedType, onlyConcreteClasses).Any();

            return SearchMethodsMatchesSearchCriteria(assemblies, func);
        }

        public IEnumerable<MethodInfo> FindMethodsOfAttribute<TAttribute>() where TAttribute : Attribute
        {
            return FindMethodsOfAttribute<TAttribute>(GetAssemblies());
        }

        public IEnumerable<MethodInfo> FindMethodsOfAttribute<TAttribute>(IEnumerable<Assembly> assemblies)
            where TAttribute : Attribute
        {
            return SearchMethodsMatchesSearchCriteria(assemblies, m => m.GetCustomAttributes<TAttribute>(true).Any());
        }

        /// <summary>Gets the assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the Nop factory.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            if (LoadAppDomainAssemblies)
                AddAssembliesInAppDomain(addedAssemblyNames, assemblies);

            AddConfiguredAssemblies(addedAssemblyNames, assemblies);

            return assemblies;
        }

        public IEnumerable<Type> FindClassesOfAttribute<TAttribute>(bool onlyConcreteClasses = true)
            where TAttribute : Attribute
        {
            return FindClassesOfAttribute<TAttribute>(GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfAttribute<TAttribute>(IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true) where TAttribute : Attribute
        {
            Func<Type, IEnumerable<Type>> func = t =>
            {
                return t.GetCustomAttributes<TAttribute>(true).Any()
                    ? ReturnTypeEnumerableWhenOnlyConcreteClassesRequired(t, onlyConcreteClasses)
                    : new Type[] {};
            };

            return SearchAssembliesBySearchCriteria(assemblies, func);
        }

        private IEnumerable<Type> ReturnTypeEnumerableWhenOnlyConcreteClassesRequired(Type type,
            bool onlyConcreteClasses)
        {
            return onlyConcreteClasses
                ? (type.IsClass && !type.IsInterface && !type.IsAbstract ? new[] {type} : new Type[] {})
                : new[] {type};
        }

        #endregion Methods

        #region Utilities

        /// <summary>
        ///     Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>
        ///     Adds specificly configured assemblies.
        /// </summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        protected virtual void AddConfiguredAssemblies(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assemblyName in AssemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
        }

        /// <summary>
        ///     Check if a dll is one of the shipped dlls that we know don'type need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        ///     The name of the assembly to check.
        /// </param>
        /// <returns>
        ///     True if the assembly should be loaded into Nop.
        /// </returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern)
                   && Matches(assemblyFullName, AssemblyRestrictToLoadingPattern);
        }

        /// <summary>
        ///     Check if a dll is one of the shipped dlls that we know don'type need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        ///     The assembly name to match.
        /// </param>
        /// <param name="pattern">
        ///     The regular expression pattern to match against the assembly name.
        /// </param>
        /// <returns>
        ///     True if the pattern matches the assembly name.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        ///     Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        /// <param name="directoryPath">
        ///     The physical path to a directory containing dlls to load in the app domain.
        /// </param>
        protected virtual void LoadMatchingAssemblies(string directoryPath)
        {
            var loadedAssemblyNames = new List<string>();
            foreach (var a in GetAssemblies())
                loadedAssemblyNames.Add(a.FullName);

            if (!Directory.Exists(directoryPath))
                return;

            foreach (var dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    if (Matches(an.FullName) && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        WatchedAppDomain.Load(an);
                    }
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        ///     Does utilType implement generic?
        /// </summary>
        /// <param name="type"></param>
        /// <param name="openGeneric"></param>
        /// <returns></returns>
        protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                var implementedInterfaces = type.FindInterfaces((objType, objCriteria) => true, null);
                foreach (var implementedInterface in implementedInterfaces)
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        protected virtual IEnumerable<MethodInfo> SearchMethodsMatchesSearchCriteria(IEnumerable<Assembly> assemblies,
            Func<MethodInfo, bool> searchCriteria)
        {
            Func<Type, IEnumerable<MethodInfo>> func =
                t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(searchCriteria);

            return SearchAssembliesBySearchCriteria(assemblies, func);
        }

        protected virtual IEnumerable<T> SearchAssembliesBySearchCriteria<T>(IEnumerable<Assembly> assemblies,
            Func<Type, IEnumerable<T>> func)
        {
            var result = new List<T>();
            try
            {
                foreach (var a in assemblies)
                {
                    Type[] types = null;
                    try
                    {
                        types = a.GetTypes();
                    }
                    catch
                    {
                        if (!ignoreReflectionErrors)
                            throw;
                    }

                    if (types != null)
                    {
                        foreach (var t in types)
                            result.AddRange(func(t));
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            return result;
        }

        #endregion Utilities
    }
}