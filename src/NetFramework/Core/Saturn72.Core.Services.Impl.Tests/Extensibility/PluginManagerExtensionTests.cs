using System;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Services.Extensibility;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Extensibility
{
    public class PluginManagerExtensionTests
    {
        [Test]
        public void PluginManagerExtensions_GetByTypeName()
        {
            var pm = new Mock<IPluginManager>();
            //Throws on illegal type name
            Should.Throw<NullReferenceException>(() => pm.Object.GetByType("eeeee"));

            PluginDescriptor pmResult = null;
            pm.Setup(p => p.GetByType(It.IsAny<Type>())).Returns(()=> pmResult);

            //return null
            pm.Object.GetByType(typeof(string).FullName).ShouldBeNull();

            //returns object
            pmResult = new PluginDescriptor();
            pm.Object.GetByType(typeof(string).FullName).ShouldBe(pmResult);
        }
    }
}
