using System;
using Moq;
using NUnit.Framework;
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
            //pm.Setup(p => p.GetByType());
            var srv = new PluginService(pm.Object);
            throw new NotImplementedException();
        }
    }
}
