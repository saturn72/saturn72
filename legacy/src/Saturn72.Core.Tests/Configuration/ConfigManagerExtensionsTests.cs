using System;
using Moq;
using Xunit;
using Saturn72.Core.Configuration;

namespace Saturn72.Core.Tests.Configuration
{
    public class ConfigManagerExtensionsTests
    {
        [Fact]
        public void ConfigManagerExtensions_GetConfig()
        {
            var cm = new Mock<IConfigManager>();
            ConfigManagerExtensions.GetConfig<ConfigObject>(cm.Object);
            cm.Verify(c=>c.GetConfig(It.Is<Type>(t=>t == typeof(ConfigObject))));
        }

        public class ConfigObject
        {
            
        }
    }
}
