#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core.Lifetime;
using Autofac.Features.Scanning;
using Saturn72.Core;
using Saturn72.Core.Exceptions;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.Ioc.Autofac
{
    public class AutofacIocContainerManager : IIocContainerManager
    {
        public IContainer Container { get; private set; }

        public virtual object Resolve(Type type, object key = null)
        {
            Func<ILifetimeScope, object> func = scope =>
                key.IsNull()
                    ? scope.Resolve(type)
                    : scope.ResolveKeyed(key, type);

            return ResolveByLifetimeScope(func);
        }

        public TService Resolve<TService>(object key = null)
        {
            Func<ILifetimeScope, TService> func = scope =>
                key.IsNull()
                    ? scope.Resolve<TService>()
                    : scope.ResolveKeyed<TService>(key);

            return ResolveByLifetimeScope(func);
        }

        public TService[] ResolveAll<TService>(object key = null)         {
            return Resolve<IEnumerable<TService>>().ToArray();
        }

        public T ResolveUnregistered<T>() where T : class
        {
            return ResolveUnregistered(typeof (T)) as T;
        }

        public object ResolveUnregistered(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null) throw new Saturn72Exception("Unknown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Saturn72Exception)
                {
                }
            }
            throw new Saturn72Exception("No constructor  was found that had all the dependencies satisfied.");
        }

        public void RegisterInstance<TService>(TService implementation, LifeCycle lifecycle, object key = null)
            where TService : class
        {
            Func<ContainerBuilder, IRegistrationBuilder<TService, IConcreteActivatorData, SingleRegistrationStyle>>
                regFunc = builder => builder.RegisterInstance(implementation).As<TService>();

            RegisterAndAssign(regFunc, lifecycle, key, typeof (TService));
        }

        public void RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null)
            where TService : class
            where TServiceImpl : TService

        {
            Func<ContainerBuilder,
                IRegistrationBuilder<TServiceImpl, ConcreteReflectionActivatorData, SingleRegistrationStyle>>
                regFunc = cb => cb.RegisterType<TServiceImpl>().As<TService>();

            RegisterAndAssign(regFunc, lifecycle, key, typeof (TService));
        }

        public void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes)
        {
            Func<ContainerBuilder,
                IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>>
                regFunc = cb => cb.RegisterTypes(serviceImplTypes);

            RegisterAndAssign(regFunc, lifeCycle, null, null);
        }

        public void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency)
        {
            Func<ContainerBuilder,
                IRegistrationBuilder<object, ReflectionActivatorData, object>>
                regFunc = cb => RegisterType(serviceImplType, cb);

            RegisterAndAssign(regFunc, lifeCycle, null, null);
        }

        public void RegisterType<TServiceImpl>(LifeCycle lifecycle = LifeCycle.PerDependency)
        {
            RegisterType(typeof (TServiceImpl), lifecycle);
        }

        public void RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle, object key = null)
        {
            Func<ContainerBuilder,
                IRegistrationBuilder<object, ReflectionActivatorData, object>>
                regFunc = cb => RegisterType(serviceImplType, cb).As(serviceType);

            RegisterAndAssign(regFunc, lifecycle, key, serviceType);
        }


        public void RegisterType(Type serviceImplType, Type[] serviceTypes, LifeCycle lifecycle)
        {
            Func<ContainerBuilder,
                IRegistrationBuilder<object, ReflectionActivatorData, object>>
                regFunc = cb => RegisterType(serviceImplType, cb).As(serviceTypes);

            RegisterAndAssign(regFunc, lifecycle, null, null);
        }


        public void Register(IEnumerable<Action<IIocRegistrator>> registerActions)
        {
            var builder = new ContainerBuilder();
            foreach (var ra in registerActions)
                ra(this);

            CreateOrUpdateContainer(builder);
        }

        public void Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle, object key = null)
        {
            //  Register( handle3)
            Func<ContainerBuilder, IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle>>
                registrationFunc =
                    builder => builder.Register(context => resolveHandler());

            RegisterAndAssign(registrationFunc, lifecycle, key, typeof (TService));
        }

        public void RegisterDelegate<TServiceImpl>(Func<IIocResolver, TServiceImpl> func, LifeCycle lifeCycle)
        {
            var reg = RegistrationBuilder.ForDelegate(
                (c, p) => func(this as IIocResolver));

            AssignByLifeCycle(reg, lifeCycle);
            var regFunc = reg.CreateRegistration();
            var regSource = new SimpleRegistrationSourceWrapper(regFunc);

            var builder = new ContainerBuilder();
            builder.RegisterSource(regSource);

            CreateOrUpdateContainer(builder);
        }

        //TODO: this is not really run in seperate scope since the resolution action will still uses the original scope to resolve.
        public void ExecuteInNewScope(Action action)
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                action();
            }
        }

        public virtual ILifetimeScope GetDefaultLifeTimeScope()
        {
            return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
        }

        public T ResolveByLifetimeScope<T>(Func<ILifetimeScope, T> func)
        {
            var scope = GetDefaultLifeTimeScope();
            return func(scope);
        }

        #region Utilities

        private static IRegistrationBuilder<object, ReflectionActivatorData, object>
            RegisterType(Type serviceImplType, ContainerBuilder cb)
        {
            if (serviceImplType.GetTypeInfo().IsGenericTypeDefinition)
                return cb.RegisterGeneric(serviceImplType);
            return cb.RegisterType(serviceImplType);
        }

        private void RegisterAndAssign<TServiceImpl>(
            Func<ContainerBuilder, IRegistrationBuilder<TServiceImpl, object, object>>
                registrationFunc, LifeCycle lifecycle, object key, Type keyedServiceType)
        {
            var builder = new ContainerBuilder();
            var reg = registrationFunc(builder);

            if (key != null)
                reg = reg.Keyed(key, keyedServiceType);

            AssignByLifeCycle(reg, lifecycle);

            CreateOrUpdateContainer(builder);
        }

        private void CreateOrUpdateContainer(ContainerBuilder containerBuilder)
        {
            if (Container.IsNull())
                Container = containerBuilder.Build();
            else
                containerBuilder.Update(Container);
        }


        private void AssignByLifeCycle<TService>(
            IRegistrationBuilder<TService, object, object> registrationBuilder,
            LifeCycle lifecycle)
        {
            //change per request to per dependency on non-web applications
            if (lifecycle == LifeCycle.PerRequest && !CommonHelper.IsWebApp())
                lifecycle = LifeCycle.PerDependency;

            switch (lifecycle)
            {
                case LifeCycle.SingleInstance:
                    registrationBuilder.SingleInstance();
                    return;
                case LifeCycle.PerRequest:
                    registrationBuilder.InstancePerRequest();
                    return;
                case LifeCycle.PerLifetime:
                    registrationBuilder.InstancePerLifetimeScope();
                    return;
                case LifeCycle.PerDependency:
                    registrationBuilder.InstancePerDependency();
                    return;
                default:
                    throw new NotImplementedException("The specified lifecycle assignment is not impelemented: " +
                                                      lifecycle.GetType().Name + "." + lifecycle);
            }
        }

        #endregion
    }
}