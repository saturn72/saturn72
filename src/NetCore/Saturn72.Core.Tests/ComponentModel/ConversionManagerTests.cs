using System;
using System.ComponentModel;
using Moq;
using Xunit;
using Saturn72.Core.Caching;
using Saturn72.Core.ComponentModel;

namespace Saturn72.Core.Tests.ComponentModel
{
    public class ConversionManagerTests
    {
        [Fact]
        public void ConversionManager_Get()
        {
            var cm = new Mock<ICacheManager>();
            var conversionManager = new ConversionManager(cm.Object);

            //Get non cached
            conversionManager.Get(typeof(string));
            cm.Verify(c => c.Get<Converter>(It.IsAny<string>()), Times.Once);
            cm.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<Converter>(), It.IsAny<int>()), Times.Once);

            //Get cached
            var cvrt = new Converter(new TypeConverter());
            cm.ResetCalls();
            cm.Setup(c => c.Get<Converter>(It.IsAny<string>())).Returns(cvrt);
            conversionManager.Get(typeof(string));
            cm.Verify(c => c.Get<Converter>(It.IsAny<string>()), Times.Once);
        }
    }
}