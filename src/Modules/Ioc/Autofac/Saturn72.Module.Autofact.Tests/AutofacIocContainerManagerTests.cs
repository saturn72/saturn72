#region

using System;
using System.Threading;
using Autofac.Core.Registration;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Module.Ioc.Autofac.Tests.TestObjects;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Module.Ioc.Autofac.Tests
{
    public class AutofacIocContainerManagerTests
    {
        [Test]
        public void RegisterDelegate_CanResolve()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterDelegate(res => DateTime.UtcNow, LifeCycle.SingleInstance);

            var startTime = DateTime.UtcNow;
            Thread.Sleep(1000);
            var dt = cm.Resolve<DateTime>();
            var totalSeconds = (int) dt.Subtract(startTime).TotalSeconds;
            totalSeconds.ShouldEqual(1);
            //var result = startTime.Subtract(dt is DateTime ? (DateTime) dt : new DateTime());

//            result.ShouldContainType(typeof(TestInterfaceImpl2));
        }

        [Test]
        public void ResolveAll_ResolvesMany()
        {
            //Resolves many
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);
            cm.RegisterType<TestInterfaceImpl2, ITestInterface1>(LifeCycle.SingleInstance);

            var result = cm.ResolveAll<ITestInterface1>();
            result.Length.ShouldEqual(2);
            result.ShouldContainType(typeof(TestInterfaceImpl1));
            result.ShouldContainType(typeof(TestInterfaceImpl2));
        }

        [Test]
        public void ResolveAll_ResolvesSingle()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);

            var result = cm.ResolveAll<ITestInterface1>();
            result.Length.ShouldEqual(1);
            result.ShouldContainType(typeof(TestInterfaceImpl1));
        }

        [Test]
        public void ResolveAll_EmptyCollectionOnNotRegistered()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);

            cm.ResolveAll<ITestInterface2>().ShouldBeEmpty();
        }

        [Test]
        public void Resolve_ResolvesType()
        {
            //Resolves many
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);
            cm.RegisterType<TestInterfaceImpl2, ITestInterface1>(LifeCycle.SingleInstance);

            var result = cm.ResolveAll<ITestInterface1>();
            result.Length.ShouldEqual(2);
            result.ShouldContainType(typeof(TestInterfaceImpl1));
            result.ShouldContainType(typeof(TestInterfaceImpl2));
        }

        [Test]
        public void Resolve_ThrowsWhenNotRegistered()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance);

            typeof(ComponentNotRegisteredException).ShouldBeThrownBy(() => cm.Resolve<ITestInterface2>());
        }

        [Test]
        public void RegisterTypeTest()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1>();
            cm.RegisterType(typeof(TestInterfaceImpl2));


            var result = cm.Resolve<TestInterfaceImpl1>();
            result.ShouldBe<TestInterfaceImpl1>();

            var result2 = cm.Resolve<TestInterfaceImpl2>();
            result2.ShouldBe<TestInterfaceImpl2>();
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
        public void RegistersKeyed()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType<TestInterfaceImpl1, ITestInterface1>(LifeCycle.SingleInstance, RegistrationKey.Online);
            cm.RegisterType<TestInterfaceImpl2, ITestInterface1>(LifeCycle.SingleInstance, RegistrationKey.Offline);
            cm.RegisterType<TestInterfaceImpl3, ITestInterface1>(LifeCycle.SingleInstance);

            var all = cm.ResolveAll<ITestInterface1>();
            all.ShouldCount(3);

            var result = cm.Resolve<ITestInterface1>(RegistrationKey.Online);
            result.ShouldBe<TestInterfaceImpl1>();

            result = cm.Resolve<ITestInterface1>(RegistrationKey.Offline);
            result.ShouldBe<TestInterfaceImpl2>();
        }

        [Test]
        public void RegisterMultipleTypes()
        {
            var cm = new AutofacIocContainerManager();
            cm.RegisterType(typeof(TestService),
                new[] {typeof(ITestService1), typeof(ITestService2)}, LifeCycle.SingleInstance);

            var result = cm.Resolve<ITestService1>();
            result.ShouldBe<TestService>();

            var result2 = cm.Resolve<ITestService2>();
            result2.ShouldBe<TestService>();
        }
    }
}