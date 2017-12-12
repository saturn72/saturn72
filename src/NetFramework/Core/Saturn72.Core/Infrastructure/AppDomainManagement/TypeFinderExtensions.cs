#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Infrastructure.AppDomainManagement
{
    public static class TypeFinderExtensions
    {
        /// <summary>
        ///     Gets all assemblies containing reuired type
        /// </summary>
        /// <typeparam name="TType">Type to scan</typeparam>
        /// <returns>
        ///     <see cref="IEnumerable{T}" />
        /// </returns>
        public static IEnumerable<Assembly> FindAssembliesWithTypeDerivatives<TType>(this ITypeFinder typeFinder)
        {
            var types = typeFinder.FindClassesOfType<TType>();

            return types.Select(x => x.Assembly).Distinct().ToArray();
        }

        /// <summary>
        ///     Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder" /></param>
        /// <param name="action">Action to run on type</param>
        public static void FindClassesOfTypeAndRunMethod<TService>(this ITypeFinder typeFinder, Action<TService> action)
        {
            FindClassesOfTypeAndRunMethod(typeFinder, action, null);
        }

        /// <summary>
        ///     Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder" /></param>
        /// <param name="action">Action to run on type</param>
        /// <param name="orderedBy">Execution order</param>
        public static void FindClassesOfTypeAndRunMethod<TService>(this ITypeFinder typeFinder, Action<TService> action,
            Func<TService, int> orderedBy)
        {
            var types = typeFinder.FindClassesOfType<TService>();

            CreateInstanceAndRunOrderedAction(types, action, orderedBy);
        }



        /// <summary>
        ///     Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder" /></param>
        /// <param name="action">Action to run on type</param>
        /// <param name="orderedBy">Execution order</param>
        /// <param name="assemblies">Assemblies collection to search for types</param>
        public static void FindClassesOfTypeAndRunMethod<TService>(this ITypeFinder typeFinder, Action<TService> action,
            Func<TService, int> orderedBy, Assembly[] assemblies)
        {
            var types = typeFinder.FindClassesOfType<TService>(assemblies);

            CreateInstanceAndRunOrderedAction(types, action, orderedBy);

        }

        private static void CreateInstanceAndRunOrderedAction<TService>(IEnumerable<Type> types, Action<TService> action, Func<TService, int> orderedBy)
        {
            var serviceList = types.Select(s => (TService)Activator.CreateInstance(s)).ToList();

            if (orderedBy.NotNull()) //sort
                serviceList = serviceList.OrderBy(orderedBy).ToList();

            foreach (var service in serviceList)
                action(service);
        }
    }
}