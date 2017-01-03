
using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Tests.TestObjects;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Tests.Infrastructure.DependencyManagement
{
    public class IocResolverExtensionsTests
    {
        [Test]
        public void TryParse_objectIsNotRegistered()
        {
            var resolver = new Mock<IIocResolver>();
            resolver.Setup(r => r.Resolve(It.IsAny<Type>(),It.IsAny<string>()))
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
                .Returns( new DummyService());

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

            throw new NotImplementedException();


            //Not Registered


            IDummyService service;
            resolver.Object.TryResolve(typeof(IDummyService), out service).ShouldBeFalse();
            service.ShouldBeNull();
        }
    }
}
