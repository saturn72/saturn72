#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Infrastructure.DependencyManagement;

#endregion

namespace Saturn72.Core.Tests.TestObjects
{
    public class DummyIocContainerManager : IIocContainerManager
    {
        public int RegisterCounter { get; set; }

        public TService Resolve<TService>(object key = null)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type, object key = null)
        {
            throw new NotImplementedException();
        }

        public TService[] ResolveAll<TService>(object key = null)
        {
            throw new NotImplementedException();
        }

        public T ResolveUnregistered<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public object ResolveUnregistered(Type type)
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<TService>(TService implementation, LifeCycle lifecycle, object key = null)
            where TService : class
        {
        }

        

        public void RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null)
            where TServiceImpl : TService where TService : class
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency)
        {
            throw new NotImplementedException();
        }

        public void RegisterType<TServiceImpl>(LifeCycle lifeCycle = LifeCycle.PerDependency)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle, object key = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type serviceImplType, Type[] serviceTypes, LifeCycle lifecycle)
        {
            throw new NotImplementedException();
        }

        public void Register(IEnumerable<Action<IIocRegistrator>> registerActions)
        {
            foreach (var action in registerActions)
            {
                RegisterCounter++;
                action(this);
            }
        }

        public void Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle, object key = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterDelegate<TServiceImpl>(Func<IIocResolver, TServiceImpl> func, LifeCycle lifeCycle)
        {
            throw new NotImplementedException();
        }

        public void ExecuteInNewScope(Action action)
        {
            throw new NotImplementedException();
        }
    }
}