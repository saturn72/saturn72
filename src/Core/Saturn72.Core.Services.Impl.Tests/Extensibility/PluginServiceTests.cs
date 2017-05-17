using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Services.Extensibility;
using Saturn72.Core.Services.Impl.Extensibility;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Extensibility
{
    public class PluginServiceTests
    {
        [Test]
        public void PluginService_GetPluginDescriptorByType_Throws()
        {
            var srv = new PluginService(null);
            Should.Throw<NullReferenceException>(() => srv.GetPluginDescriptorByType(null));
        }

        [Test]
        public void PluginService_ReturnsPluginDescriptor()
        {

            var pm = new Mock<IPluginManager>();
            var pmResult = new PluginDescriptor();
            pm.Setup(p => p.GetByType(It.IsAny<Type>())).Returns(pmResult);

            var srv = new PluginService(pm.Object);
            srv.GetPluginDescriptorByType(typeof(string)).ShouldBe(pmResult);
        }

        [Test]
        public void PluginService_ReturnsNull()
        {
            var pm = new Mock<IPluginManager>();
            pm.Setup(p => p.GetByType(It.IsAny<Type>())).Returns((PluginDescriptor)null);

            var srv = new PluginService(pm.Object);
            srv.GetPluginDescriptorByType(typeof(string)).ShouldBe(null);
        }
    }
}
