using System;
using Shouldly;
using Xunit;
using Moq;
using Saturn72.Core.Caching;

namespace Saturn72.Core.Tests.Caching
{
    public class CacheManagerExtensionsTests
    {
        [Fact]
        public void CacheManagerExtensions_Set()
        {
            var cm = new Mock<ICacheManager>();
            var key = "key";
            var value = new CacheTestObject();
            CacheManagerExtensions.Set(cm.Object, key, value);
            cm.Verify(c => c.Set(
                It.Is<string>(k => k == key),
                It.Is<CacheTestObject>(cto => cto == value),
                It.Is<uint>(i => i == CacheManagerExtensions.DefaultCacheTime)), Times.Once);
        }

        [Fact]
        public void CacheManagerExtensions_SetIfNotExists()
        {
            var cm = new Mock<ICacheManager>();

            var key = "key";
            var value = new CacheTestObject();
            var cacheTime = 123U;

            //object is cached
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(null as CacheTestObject);

            CacheManagerExtensions.SetIfNotExists(cm.Object, key, value, cacheTime);
            cm.Verify(c => c.Set(
                    It.Is<string>(k => k == key),
                    It.Is<CacheTestObject>(cto => cto == value),
                    It.IsAny<uint>()),
                Times.Once);

            //object is not cached
            cm.Reset();
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(new CacheTestObject());
            CacheManagerExtensions.SetIfNotExists(cm.Object, key, value, cacheTime);
            cm.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<CacheTestObject>(), It.IsAny<uint>()),
                Times.Never);
        }

        [Fact]
        public void CacheManagerExtensions_GetOrAquire()
        {
            var cm = new Mock<ICacheManager>();
            var key = "key";
            var cacheTime = 123U;
            var wasAquired = false;
            Func<CacheTestObject> aquireFunc = () =>
            {
                wasAquired = true;
                return new CacheTestObject();
            };
            //does not call aquire func
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(new CacheTestObject());

            CacheManagerExtensions.Get(cm.Object, key, aquireFunc, cacheTime);
            wasAquired.ShouldBeFalse();

            //should call call aquire func
            cm.Setup(c => c.Get<CacheTestObject>(It.IsAny<string>())).Returns(null as CacheTestObject);
            CacheManagerExtensions.Get(cm.Object, key, aquireFunc, cacheTime);
            wasAquired.ShouldBeTrue();
        }

        internal class CacheTestObject
        {

        }
    }
}
