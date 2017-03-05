using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Tests.Infrastructure.DependencyManagement
{
    public class IocResolverExtensionsTests
    {
        [Test]
        public void TryParse_Isregistered()
        {
            var resolver = new Mock<IIocResolver>();

            //false
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<object>())).Returns(null);
            resolver.Object.IsRegistered(typeof(string)).ShouldBeFalse();
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<object>())).Throws<NullReferenceException>();
            resolver.Object.IsRegistered(typeof(string)).ShouldBeFalse();

            //true
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<object>())).Returns("dadata");
            resolver.Object.IsRegistered(typeof(string)).ShouldBeTrue();
        }

        [Test]
        public void TryParse_objectIsNotRegistered()
        {
            var resolver = new Mock<IIocResolver>();
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<string>()))
                .Throws<NullReferenceException>();

            //Not Registered
            resolver.Object.TryResolve<IDummyService>(typeof(IDummyService)).ShouldBeNull();


            IDummyService service;
            resolver.Object.TryResolve(typeof(IDummyService), out service).ShouldBeFalse();
            service.ShouldBeNull();
        }

        [Test]
        public void TryParse_objectIsRegistered()
        {
            var resolver = new Mock<IIocResolver>();
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(new DummyService(null));

            //Not Registered
            resolver.Object.TryResolve<IDummyService>(typeof(IDummyService)).GetType().ShouldBeType<DummyService>();

            IDummyService service;
            resolver.Object.TryResolve(typeof(IDummyService), out service).ShouldBeTrue();
            service.ShouldNotBeNull();
            service.GetType().ShouldBeType<DummyService>();
        }

        [Test]
        public void TryParse_ResolveUnregistered_Fails()
        {
            var resolver = new Mock<IIocResolver>();
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<string>()))
                .Throws<NullReferenceException>();

            resolver.Object.TryResolve<IDummyService>(typeof(DummyService)).ShouldBeNull();
            resolver.Object.TryResolve<DummyService>(typeof(DummyService)).ShouldBeNull();

            IDummyService service;
            resolver.Object.TryResolve(typeof(IDummyService), out service).ShouldBeFalse();
            service.ShouldBeNull();

            resolver.Object.TryResolve(typeof(DummyService), out service).ShouldBeFalse();
            service.ShouldBeNull();
        }

        [Test]
        public void TryParse_ResolveUnregistered_Resolves()
        {
            var resolver = new Mock<IIocResolver>();
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(), It.IsAny<string>()))
                .Throws<NullReferenceException>();

            resolver.Object.TryResolve<IDummyService>(typeof(DummyService1)).ShouldNotBeNull();
            resolver.Object.TryResolve<DummyService1>(typeof(DummyService1)).ShouldNotBeNull();
            IDummyService service;
            resolver.Object.TryResolve(typeof(DummyService1), out service).ShouldBeTrue();

            service.ShouldNotBeNull();
        }
    }

    public class DummyService1 : IDummyService
    {
    }


    public class DummyService : IDummyService
    {
        public DummyService(ICollection<string> col)
        {
        }
    }

    public interface IDummyService
    {
    }
}