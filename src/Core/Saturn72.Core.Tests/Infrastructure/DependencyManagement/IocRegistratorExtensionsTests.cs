using Moq;
using NUnit.Framework;
using Saturn72.Core.Infrastructure.DependencyManagement;

namespace Saturn72.Core.Tests.Infrastructure.DependencyManagement
{
    [Category("unit_test")]
    public class IocRegistratorExtensionsTests
    {
        [Test]
        public void IocRegistratorExtensions_RegisterType_MultiInterfaces()
        {
            var i = 0;
            var reg = new Mock<IIocRegistrator>();
            var lc = LifeCycle.SingleInstance;
            reg.Object.RegisterType(typeof(TestObject),
                new[] {typeof(ITestObject1), typeof(ITestObject2), typeof(ITestObject3)}, lc);


            reg.Verify(r => r.RegisterType(typeof(TestObject), typeof(ITestObject1), lc, null, null), Times.Once);
            reg.Verify(r => r.RegisterType(typeof(TestObject), typeof(ITestObject2), lc, null, null), Times.Once);
            reg.Verify(r => r.RegisterType(typeof(TestObject), typeof(ITestObject3), lc, null, null), Times.Once);
        }

        [Test]
        public void IocRegistratorExtensions_RegisterType_MultiInterfaces_WithDuplication()
        {
            var i = 0;
            var reg = new Mock<IIocRegistrator>();
            var lc = LifeCycle.SingleInstance;
            reg.Object.RegisterType(typeof(TestObject),
                new[] {typeof(ITestObject1), typeof(ITestObject2), typeof(ITestObject2), typeof(ITestObject3)}, lc);


            reg.Verify(r => r.RegisterType(typeof(TestObject), typeof(ITestObject1), lc, null, null), Times.Once);
            reg.Verify(r => r.RegisterType(typeof(TestObject), typeof(ITestObject2), lc, null, null), Times.Once);
            reg.Verify(r => r.RegisterType(typeof(TestObject), typeof(ITestObject3), lc, null, null), Times.Once);
        }

        internal class TestObject : ITestObject1, ITestObject2, ITestObject3
        {
        }
    }

    public interface ITestObject2
    {
    }

    public interface ITestObject3
    {
    }

    public interface ITestObject1
    {
    }
}