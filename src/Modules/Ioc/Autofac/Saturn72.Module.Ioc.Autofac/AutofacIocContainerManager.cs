﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Activators.ProvidedInstance;
using Autofac.Core.Lifetime;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.Scanning;
using Autofac.Integration.WebApi;
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

            return Resolve(func);
        }

        public virtual TService Resolve<TService>(object key = null)
        {
            return (TService) Resolve(typeof(TService), key);
        }

        public virtual TService[] ResolveAll<TService>(object key = null)
        {
            return Resolve<IEnumerable<TService>>().ToArray();
        }

        public T ResolveUnregistered<T>() where T : class
        {
            return ResolveUnregistered(typeof(T)) as T;
        }

        public object ResolveUnregistered(Type type)
        {
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
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
            throw new Saturn72Exception("No constructor  was found that had all the dependencies satisfied.");
        }

        public IocRegistrationRecord RegisterInstance<TService>(TService implementer, object key = null,
            Type[] interceptorTypes = null)
            where TService : class
        {
            Func<ContainerBuilder, IRegistrationBuilder<TService, IConcreteActivatorData, SingleRegistrationStyle>>
                regFunc = builder => interceptorTypes.NotEmptyOrNull()
                    ? builder.RegisterInstance(implementer)
                        .As<TService>()
                        .EnableInterfaceInterceptors()
                        .InterceptedBy(interceptorTypes)
                    : builder.RegisterInstance(implementer).As<TService>();

            return ToIocRegistrationRecord(RegisterAndAssign(regFunc, LifeCycle.SingleInstance, key, typeof(TService)));
        }

        public IocRegistrationRecord RegisterType<TServiceImpl, TService>(LifeCycle lifecycle, object key = null,
            Type[] interceptorTypes = null)
            where TService : class
            where TServiceImpl : TService

        {
            Func
                <ContainerBuilder,
                    IRegistrationBuilder<TServiceImpl, ConcreteReflectionActivatorData, SingleRegistrationStyle>>
                regFunc = cb => interceptorTypes.NotEmptyOrNull()
                    ? cb.RegisterType<TServiceImpl>()
                        .As<TService>()
                        .EnableInterfaceInterceptors()
                        .InterceptedBy(interceptorTypes)
                    : cb.RegisterType<TServiceImpl>().As<TService>();

            return ToIocRegistrationRecord(RegisterAndAssign(regFunc, lifecycle, key, typeof(TService)));
        }

        public void RegisterTypes(LifeCycle lifeCycle, params Type[] serviceImplTypes)
        {
            Func<ContainerBuilder,
                    IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>>
                regFunc = cb => cb.RegisterTypes(serviceImplTypes);

            RegisterAndAssign(regFunc, lifeCycle, null, null);
        }

        public void RegisterType(Type serviceImplType, LifeCycle lifeCycle = LifeCycle.PerDependency,
            Type[] interceptorTypes = null)
        {
            Func<ContainerBuilder, IRegistrationBuilder<object, ReflectionActivatorData, object>>
                regFunc = cb => RegisterType(serviceImplType, cb, interceptorTypes);

            RegisterAndAssign(regFunc, lifeCycle, null, null);
        }

        public void RegisterType<TServiceImpl>(LifeCycle lifecycle = LifeCycle.PerDependency)
        {
            RegisterType(typeof(TServiceImpl), lifecycle);
        }

        public IocRegistrationRecord RegisterType(Type serviceImplType, Type serviceType, LifeCycle lifecycle,
            object key = null, Type[] interceptorTypes = null)
        {
            Func<ContainerBuilder, IRegistrationBuilder<object, ReflectionActivatorData, object>>
                regFunc = cb => interceptorTypes.NotEmptyOrNull()
                    ? RegisterType(serviceImplType, cb)
                        .As(serviceType)
                        .EnableInterfaceInterceptors()
                        .InterceptedBy(interceptorTypes)
                    : RegisterType(serviceImplType, cb).As(serviceType);

            return ToIocRegistrationRecord(RegisterAndAssign(regFunc, lifecycle, key, serviceType));
        }


        public IocRegistrationRecord RegisterType(Type serviceImplType, Type[] serviceTypes, LifeCycle lifecycle,
            Type[] interceptorTypes = null)
        {
            Func<ContainerBuilder,
                    IRegistrationBuilder<object, ReflectionActivatorData, object>>
                regFunc = cb => interceptorTypes.NotEmptyOrNull()
                    ? RegisterType(serviceImplType, cb)
                        .As(serviceTypes)
                        .EnableInterfaceInterceptors()
                        .InterceptedBy(interceptorTypes)
                    : RegisterType(serviceImplType, cb).As(serviceTypes);

            return ToIocRegistrationRecord(RegisterAndAssign(regFunc, lifecycle, null, null));
        }


        public void Register(IEnumerable<Action<IIocRegistrator>> registerActions)
        {
            var builder = new ContainerBuilder();
            foreach (var ra in registerActions)
                ra(this);

            CreateOrUpdateContainer(builder);
        }

        public IocRegistrationRecord Register<TService>(Func<TService> resolveHandler, LifeCycle lifecycle,
            object key = null, Type[] interceptorTypes = null)
        {
            Func<ContainerBuilder, IRegistrationBuilder<TService, SimpleActivatorData, SingleRegistrationStyle>>
                registrationFunc =
                    cb => interceptorTypes.NotEmptyOrNull()
                        ? cb.Register(context => resolveHandler())
                            .EnableInterfaceInterceptors()
                            .InterceptedBy(interceptorTypes)
                        : cb.Register(context => resolveHandler());

            return ToIocRegistrationRecord(RegisterAndAssign(registrationFunc, lifecycle, key, typeof(TService)));
        }

        public void RegisterDelegate<TService>(Func<IIocResolver, TService> func, LifeCycle lifeCycle,
            Type[] interceptorTypes = null)
        {
            var reg = interceptorTypes.NotEmptyOrNull()
                ? RegistrationBuilder.ForDelegate((c, p) => func(this))
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(interceptorTypes)
                : RegistrationBuilder.ForDelegate((c, p) => func(this));

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

        public IocRegistrationRecord RegisterGeneric(Type implementerType, Type serviceType, LifeCycle lifeCycle,
            object key = null,
            Type[] interceptorTypes = null)
        {
            Func<ContainerBuilder, IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>>
                registrationFunc = cb => interceptorTypes.NotEmptyOrNull()
                    ? cb.RegisterGeneric(implementerType)
                        .As(serviceType)
                        .EnableInterfaceInterceptors()
                        .InterceptedBy(interceptorTypes)
                    : cb.RegisterGeneric(implementerType).As(serviceType);
            return ToIocRegistrationRecord(RegisterAndAssign(registrationFunc, lifeCycle, key, serviceType));
        }

        // private IocRegistrationRecord ToIocRegistrationRecord<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> regBuilder)
        private IocRegistrationRecord ToIocRegistrationRecord<TService, TActivatorData, TRegistrationStyle>(
            IRegistrationBuilder<TService, TActivatorData, TRegistrationStyle> regBuilder)
        {
            var activatorType = ExtractActivatorType(regBuilder.ActivatorData);
            var ImplType = (regBuilder.ActivatorData as ConcreteReflectionActivatorData)?.ImplementationType ??
                           (regBuilder.ActivatorData as ReflectionActivatorData)?.ImplementationType ??
                           (regBuilder.ActivatorData as SimpleActivatorData)?.Activator.LimitType;

            var serviceTypes = GetAllServiceTypes(regBuilder.RegistrationData.Services);

            var keys = regBuilder.RegistrationData.Services.Where(s => s is KeyedService)
                .Select(srv => (srv as KeyedService).ServiceKey).ToArray();

            return new IocRegistrationRecord
            {
                Metadata = regBuilder.RegistrationData.Metadata,
                ServiceTypes = serviceTypes,
                ImplementedType = ImplType,
                RegistrationId = (regBuilder.RegistrationStyle as SingleRegistrationStyle)?.Id.ToString(),
                ActivatorType = activatorType,
                Keys = keys
            };
        }

        private Type[] GetAllServiceTypes(IEnumerable<Service> regDataServices)
        {
            return regDataServices
                .Select(s => (s as TypedService)?.ServiceType ?? (s as KeyedService)?.ServiceType ?? s.GetType())
                .GroupBy(grp => grp)
                .Select(g => g.First())
                .ToArray();
        }

        private ActivatorType ExtractActivatorType(object activatorData)
        {
            if (activatorData is ConcreteReflectionActivatorData || activatorData is ReflectionActivatorData)
                return ActivatorType.Constractor;

            var actData = activatorData as SimpleActivatorData;
            if (actData != null)
                return actData.Activator is ProvidedInstanceActivator ? ActivatorType.Instance : ActivatorType.Delegate;

            throw new NotSupportedException("The activator type is not supported");
        }

        public virtual T Resolve<T>(Func<ILifetimeScope, T> func)
        {
            var scope = BeginLifetimeScope();
            return func(scope);
        }

        protected virtual ILifetimeScope BeginLifetimeScope()
        {
            return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
        }

        #region Utilities

        private IRegistrationBuilder<object, ReflectionActivatorData, object> RegisterType(Type serviceImplType,
            ContainerBuilder cb, Type[] interceptorTypes = null)
        {
            if (serviceImplType.GetTypeInfo().IsGenericTypeDefinition)
                return interceptorTypes.NotEmptyOrNull()
                    ? cb.RegisterGeneric(serviceImplType).EnableInterfaceInterceptors().InterceptedBy(interceptorTypes)
                    : cb.RegisterGeneric(serviceImplType);
            return interceptorTypes.NotEmptyOrNull()
                ? cb.RegisterType(serviceImplType).EnableClassInterceptors().InterceptedBy(interceptorTypes)
                : cb.RegisterType(serviceImplType);
        }

        private IRegistrationBuilder<TServiceImpl, object, object> RegisterAndAssign<TServiceImpl>(
            Func<ContainerBuilder, IRegistrationBuilder<TServiceImpl, object, object>>
                registrationFunc, LifeCycle lifecycle, object key, Type keyedServiceType)

        {
            var builder = new ContainerBuilder();
            var reg = registrationFunc(builder);

            if (key != null)
                reg = reg.Keyed(key, keyedServiceType);

            AssignByLifeCycle(reg, lifecycle);

            CreateOrUpdateContainer(builder);

            return reg;
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