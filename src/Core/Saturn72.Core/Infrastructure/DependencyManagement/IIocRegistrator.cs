#region

using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

#endregion

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IIocRegistrator
    {
        IocRegistrationRecord RegisterInstance<TService>(TService implementation, object key = null, Type interceptorType = null)
            where TService : class;

        IocRegistrationRecord RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null, Type interceptorType = null)
            where TService : class
            where TServiceImpl : TService;

        void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes);

        void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency, Type interceptorType = null);

        void RegisterType<TServiceImpl>(LifeCycle lifeCycle = LifeCycle.PerDependency);

        IocRegistrationRecord RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle, object key = null, Type interceptorType = null);

        IocRegistrationRecord RegisterType(Type serviceImplType, Type[] serviceTypes, LifeCycle lifecycle, Type interceptorType = null);

        void Register(IEnumerable<Action<IIocRegistrator>> registerActions);

        IocRegistrationRecord Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle, object key = null, Type interceptorType = null);

        void RegisterDelegate<TService>(Func<IIocResolver, TService> func, LifeCycle lifeCycle, Type interceptorType = null);
    }
}