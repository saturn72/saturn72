#region

using System;
using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IIocRegistrator
    {
        void RegisterInstance<TService>(TService implementation, LifeCycle lifecycle, object key = null)
            where TService : class;

        void RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null)
            where TService : class
            where TServiceImpl : TService;

        void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes);

        void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency);

        void RegisterType<TServiceImpl>(LifeCycle lifeCycle = LifeCycle.PerDependency);

        void RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle, object key = null);

        void RegisterType(Type serviceImplType, Type[] serviceTypes, LifeCycle lifecycle);

        void Register(IEnumerable<Action<IIocRegistrator>> registerActions);

        void Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle, object key = null);

        void RegisterDelegate<TServiceImpl>(Func<IIocResolver, TServiceImpl> func, LifeCycle lifeCycle);
    }
}