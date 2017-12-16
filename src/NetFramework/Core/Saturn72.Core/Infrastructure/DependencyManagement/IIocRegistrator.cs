#region

using System;
using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IIocRegistrator
    {
        IocRegistrationRecord RegisterInstance<TService>(TService implementer, object key = null, Type[] interceptorTypes = null)
            where TService : class;

        IocRegistrationRecord RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null, Type[] interceptorTypes = null)
            where TService : class
            where TServiceImpl : TService;

        void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes);

        void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency, Type[] interceptorTypes = null);

        void RegisterType<TServiceImpl>(LifeCycle lifeCycle = LifeCycle.PerDependency);

        IocRegistrationRecord RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle, object key = null, Type[] interceptorTypes = null);
        void Register(IEnumerable<Action<IIocRegistrator>> registerActions);

        IocRegistrationRecord Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle, object key = null, Type[] interceptorTypes = null);

        void RegisterDelegate<TService>(Func<IIocResolver, TService> func, LifeCycle lifeCycle, Type[] interceptorTypes = null);

        IocRegistrationRecord RegisterGeneric(Type implementerType, Type serviceType, LifeCycle lifeCycle,
            object key = null,
            Type[] interceptorTypes = null);
    }
}