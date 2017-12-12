#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Infrastructure.DependencyManagement;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.TestObjects
{
    public class DummyIocContainerManager : IIocContainerManager
    {
        private readonly IDictionary<Type, ICollection<Func<object>>> _registrations =
            new Dictionary<Type, ICollection<Func<object>>>();

        public int RegisterCounter { get; set; }

        public object Resolve(Type type, object key = null)
        {
            throw new NotImplementedException();
        }


        public IocRegistrationRecord RegisterInstance<TService>(TService implementer, object key = null,
            Type[] interceptorTypes = null) where TService : class
        {
            var serviceType = typeof(TService);
            if (!_registrations.ContainsKey(typeof(TService)))
                _registrations[serviceType] = new List<Func<object>>();

            _registrations[serviceType].Add(() => implementer);
            return new IocRegistrationRecord();
        }

        public IocRegistrationRecord RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null,
            Type[] interceptorTypes = null) where TServiceImpl : TService where TService : class
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes)
        {
            throw new NotImplementedException();
        }

        public void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency,
            Type[] interceptorTypes = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterType<TServiceImpl>(LifeCycle lifeCycle = LifeCycle.PerDependency)
        {
            throw new NotImplementedException();
        }

        public IocRegistrationRecord RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle,
            object key = null,
            Type[] interceptorTypes = null)
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

        public IocRegistrationRecord Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle,
            object key = null,
            Type[] interceptorTypes = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterDelegate<TService>(Func<IIocResolver, TService> func, LifeCycle lifeCycle,
            Type[] interceptorTypes = null)
        {
            throw new NotImplementedException();
        }

        public IocRegistrationRecord RegisterGeneric(Type implementerType, Type serviceType, LifeCycle lifeCycle,
            object key = null,
            Type[] interceptorTypes = null)
        {
            throw new NotImplementedException();
        }

        public void ExecuteInNewScope(Action action)
        {
            throw new NotImplementedException();
        }

        public TService Resolve<TService>(object key = null)
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

        public IocRegistrationRecord RegisterInstance<TService>(TService implementation, object key = null,
            Type interceptorType = null)
            where TService : class
        {
            return null;
        }
    }
}