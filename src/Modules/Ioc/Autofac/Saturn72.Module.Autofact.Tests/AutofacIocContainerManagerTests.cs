#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac.Core.Registration;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Module.Ioc.Autofac.Tests.TestObjects;
using Shouldly;

#endregion

namespace Saturn72.Module.Ioc.Autofac.Tests
{
    [TestFixture]
    public class AutofacIocContainerManagerTests
    {
        [Test]
        public void Register_ReturnsIocRegistrationRecord()
        {
            var cm = new AutofacIocContainerManager();

            //register type returns ioc
            var regRecord = cm.RegisterType<TestService, ITestService1>(LifeCycle.SingleInstance);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService1));
            regRecord.ImplementedType.ShouldBe(typeof(TestService));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Constractor);

            //Now keyed
            var key = "TTT";
            regRecord = cm.RegisterType<TestService2, ITestService2>(LifeCycle.SingleInstance, key);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService2));
            regRecord.ImplementedType.ShouldBe(typeof(TestService2));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Constractor);
            regRecord.Keys.Count().ShouldBe(1);
            regRecord.Keys.First().ShouldBe(key);


            //register type returns ioc
            regRecord = cm.RegisterType(typeof(TestService), typeof(ITestService1), LifeCycle.SingleInstance);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService1));
            regRecord.ImplementedType.ShouldBe(typeof(TestService));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Constractor);

            //keyed
            regRecord = cm.RegisterType(typeof(TestService), typeof(ITestService1), LifeCycle.SingleInstance, key);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService1));
            regRecord.ImplementedType.ShouldBe(typeof(TestService));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Constractor);
            regRecord.Keys.Count().ShouldBe(1);
            regRecord.Keys.First().ShouldBe(key);

            //register single instance
            var srv3 = new TestService3();
            regRecord = cm.RegisterInstance<ITestService3>(srv3);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService3));
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ImplementedType.ShouldBe(typeof(TestService3));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Instance);

            //keyed
            regRecord = cm.RegisterInstance<ITestService3>(srv3, key);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService3));
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ImplementedType.ShouldBe(typeof(TestService3));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Instance);
            regRecord.Keys.Count().ShouldBe(1);
            regRecord.Keys.First().ShouldBe(key);
            //register 

            regRecord = cm.Register<ITestService3>(() => new TestService3(), LifeCycle.PerLifetime);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService3));
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ImplementedType.ShouldBe(typeof(ITestService3));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Delegate);

            //Keyed
            regRecord = cm.Register<ITestService3>(() => new TestService3(), LifeCycle.PerLifetime, key);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(ITestService3));
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ImplementedType.ShouldBe(typeof(ITestService3));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Delegate);
            regRecord.Keys.Count().ShouldBe(1);
            regRecord.Keys.First().ShouldBe(key);


            //register generic 
            regRecord = cm.RegisterGeneric(typeof(GenericService<>), typeof(IGenericService<>), LifeCycle.PerLifetime);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(IGenericService<>));
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ImplementedType.ShouldBe(typeof(GenericService<>));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Constractor);
            regRecord.Keys.Count().ShouldBe(0);

            //register generic keyed
            regRecord = cm.RegisterGeneric(typeof(GenericService<>), typeof(IGenericService<>), LifeCycle.PerLifetime,
                key);
            regRecord.Metadata.ShouldNotBeNull();
            regRecord.ServiceTypes.Count().ShouldBe(1);
            regRecord.ServiceTypes.First().ShouldBe(typeof(IGenericService<>));
            regRecord.RegistrationId.ShouldNotBe(Guid.Empty.ToString());
            regRecord.ImplementedType.ShouldBe(typeof(GenericService<>));
            regRecord.ActivatorType.ShouldBe(ActivatorType.Constractor);
            regRecord.Keys.Count().ShouldBe(1);
            regRecord.Keys.First().ShouldBe(key);
        }

        [Test]
        public void RegisterDelegate_CanResolve()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterDelegate(res => DateTime.UtcNow, LifeCycle.SingleInstance);

            var startTime = DateTime.UtcNow;
            Thread.Sleep(1000);
            var dt = cm.Resolve<DateTime>();
            var totalSeconds = (int) dt.Subtract(startTime).TotalSeconds;
            totalSeconds.ShouldBe(1);
        }

        [Test]
        public void Registergeneric_CanResolve()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterGeneric(typeof(GenericService<>), typeof(IGenericService<>), LifeCycle.PerDependency);
            cm.Resolve<IGenericService<string>>().GetType().ShouldBe(typeof(GenericService<string>));
        }

        [Test]
        public void RegisterMultipleTypes()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType(typeof(TestService),
                new[] {typeof(ITestService1), typeof(ITestService2)}, LifeCycle.SingleInstance);

            var result = cm.Resolve<ITestService1>();
            result.ShouldBeOfType<TestService>();

            var result2 = cm.Resolve<ITestService2>();
            result2.ShouldBeOfType<TestService>();
        }

        [Test]
        public void RegistersKeyed()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance, RegistrationKey.Online);
            cm.RegisterType<TestInterfaceImpl2, ITestInterface1>(LifeCycle.SingleInstance, RegistrationKey.Offline);
            cm.RegisterType<TestInterfaceImpl3, ITestInterface1>(LifeCycle.SingleInstance);

            var all = cm.ResolveAll<ITestInterface1>();
            all.Length.ShouldBe(3);

            var result = cm.Resolve<ITestInterface1>(RegistrationKey.Online);
            result.ShouldBeOfType<TestInterfaceImpl1>();

            result = cm.Resolve<ITestInterface1>(RegistrationKey.Offline);
            result.ShouldBeOfType<TestInterfaceImpl2>();
        }

        [Test]
        public void RegisterTypes_CanResolve()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterTypes(LifeCycle.PerDependency, typeof(TestInterfaceImpl1), typeof(TestInterfaceImpl2),
                typeof(TestInterfaceImpl3));

            var result = cm.Resolve<TestInterfaceImpl1>();
            Assert.IsInstanceOf<TestInterfaceImpl1>(result);

            var result2 = cm.Resolve<TestInterfaceImpl2>();
            Assert.IsInstanceOf<TestInterfaceImpl2>(result2);

            var result3 = cm.Resolve<TestInterfaceImpl3>();
            Assert.IsInstanceOf<TestInterfaceImpl3>(result3);
        }

        [Test]
        public void RegisterTypeTest()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1>();
            cm.RegisterType(typeof(TestInterfaceImpl2));


            var result = cm.Resolve<TestInterfaceImpl1>();
            result.ShouldBeOfType<TestInterfaceImpl1>();

            var result2 = cm.Resolve<TestInterfaceImpl2>();
            result2.ShouldBeOfType<TestInterfaceImpl2>();
        }

        [Test]
        public void Resolve_ResolvesType()
        {
            //Resolves many
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);
            cm.RegisterType<TestInterfaceImpl2, ITestInterface1>(LifeCycle.SingleInstance);

            var result = cm.ResolveAll<ITestInterface1>();
            result.Length.ShouldBe(2);
            result.ShouldContain(t=>t.GetType() == typeof(TestInterfaceImpl1));
            result.ShouldContain(t => t.GetType() == typeof(TestInterfaceImpl2));
        }

        [Test]
        public void Resolve_ThrowsWhenNotRegistered()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);

           Should.Throw<ComponentNotRegisteredException>(() => cm.Resolve<ITestInterface2>());
        }

        [Test]
        public void ResolveAll_EmptyCollectionOnNotRegistered()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);

            cm.ResolveAll<ITestInterface2>().ShouldBeEmpty();
        }


        [Test]
        public void ResolveAll_ResolvesMany()
        {
            //Resolves many
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);
            cm.RegisterType<TestInterfaceImpl2, ITestInterface1>(LifeCycle.SingleInstance);

            var result = cm.ResolveAll<ITestInterface1>();
            result.Length.ShouldBe(2);
            result.ShouldContain(t => t.GetType() == typeof(TestInterfaceImpl1));
            result.ShouldContain(t => t.GetType() == typeof(TestInterfaceImpl2));
        }

        [Test]
        public void ResolveAll_ResolvesSingle()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);

            var result = cm.ResolveAll<ITestInterface1>();
            result.Length.ShouldBe(1);
            result.ShouldContain(t => t.GetType() == typeof(TestInterfaceImpl1));
        }
    }
}