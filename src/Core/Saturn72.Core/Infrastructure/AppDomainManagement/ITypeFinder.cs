#region

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Saturn72.Core.Infrastructure.AppDomainManagement
{
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();
        IEnumerable<Type> FindClassesOfType<TType>(bool onlyConcreteClasses = true);
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<TType>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfAttribute<TAttribute>(bool onlyConcreteClasses = true)
            where TAttribute : Attribute;

        IEnumerable<Type> FindClassesOfAttribute<TAttribute>(IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true) where TAttribute : Attribute;

        /// <summary>
        ///     Get all methods by return utilType
        /// </summary>
        /// <typeparam name="TReturnType">Method return utilType</typeparam>
        /// <returns>Invocation[]</returns>
        IEnumerable<MethodInfo> FindMethodsOfReturnType<TReturnType>(bool onlyConcreteClasses = true);

        /// <summary>
        ///     Get all methods by return utilType
        /// </summary>
        /// <returns>Invocation[]</returns>
        IEnumerable<MethodInfo> FindMethodsOfReturnType(Type returnType, bool onlyConcreteClasses = true);

        /// <summary>
        ///     Get all methods by return utilType
        /// </summary>
        /// <param name="returnType"></param>
        /// <param name="assemblies"></param>
        /// <param name="onlyConcreteClasses"></param>
        /// <returns>Invocation[]</returns>
        IEnumerable<MethodInfo> FindMethodsOfReturnType(Type returnType, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true);

        /// <summary>
        ///     Get all methods decorated with attributeType
        /// </summary>
        /// <typeparam name="TAttribute">Attribute.</typeparam>
        /// <returns>Invocation[]</returns>
        IEnumerable<MethodInfo> FindMethodsOfAttribute<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        ///     Return all methods decorated with attribute
        /// </summary>
        /// <typeparam name="TAttribute">Attribute Type</typeparam>
        /// <param name="assemblies">Assemblies collection</param>
        /// <returns></returns>
        IEnumerable<MethodInfo> FindMethodsOfAttribute<TAttribute>(IEnumerable<Assembly> assemblies)
            where TAttribute : Attribute;
    }
}