
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Shouldly;

namespace Saturn72.Core.Tests.Caching
{
    public class CacheManagerExtensionsTests
    {
        [Test]
        public void CacheManage_Set()
        {
            object t = null;
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((s, o, i) => t = i);

            var value = "value";
            cm.Object.Set("key", value);
            t.ShouldBe(60);
        }

        [Test]
        public void CacheManage_Get_Aquire_NewItem()
        {
            object t = null;
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((s, o, i) => t = o);

            var value = "value";
            cm.Object.Get("key", () => value);
            t.ShouldBe(value);
        }

        [Test]
        public void CacheManage_Get_AquireCachedItem()
        {
            string key = "key", value = "value";
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Keys)
                .Returns(new[] {key});

            cm.Setup(c => c.Get<string>(It.IsAny<string>()))
                .Returns(value);

            cm.Object.Get(key, () => "AAA").ShouldBe(value);
        }

        [Test]
        public void CacheManage_Get_Async_Aquire_NewItem()
        {
            object t = null;
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((s, o, i) => t = o);

            var value = "value";
            cm.Object.Get("key", () => Task.Run<string>(() =>
            {
                Thread.Sleep(10);
                return value;
            })).Result.ShouldBe(value);
        }

        [Test]
        public void CacheManage_Get_Async_AquireCachedItem()
        {
            string key = "key", value = "value";
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Keys)
                .Returns(new[] {key});

            cm.Setup(c => c.Get<string>(It.IsAny<string>()))
                .Returns(value);

            cm.Object.Get(key, () => Task.Run<string>(() =>
            {
                Thread.Sleep(10);
                return value;
            })).Result.ShouldBe(value);
        }

        [Test]
        public void CacheManger_IsSet_ReturnsFalse()
        {
            var cm = new TestMemoryCacheManager();
            //on empty cache
            cm.IsSet("123").ShouldBeFalse();

            //on filled cache
            var key1 = "key1";
            cm.TempCache[key1] = "value1";

            cm.IsSet("123").ShouldBeFalse();
        }

        [Test]
        public void CacheManger_IsSet_ReturnsTrue()
        {
            var cm = new TestMemoryCacheManager();
            var key1 = "key1";
            cm.TempCache[key1] = "value1";
            cm.IsSet(key1).ShouldBeTrue();
        }

        [Test]
        public void CacheManger_RemoveByPattern_RemovesNone()
        {
            var cm = new TestMemoryCacheManager();
            cm.TempCache["key1"] = "value1";
            cm.TempCache["key2"] = "value1";
            cm.RemoveByPattern("w");
            cm.Keys.Count().ShouldBe(2);
        }

        [Test]
        public void CacheManger_RemoveByPattern_RemovesAll()
        {
            var cm = new TestMemoryCacheManager();
            cm.TempCache["key1"] = "value1";
            cm.TempCache["key2"] = "value1";
            cm.RemoveByPattern("key");
            cm.Keys.Count().ShouldBe(0);
        }

        [Test]
        public void CacheManger_SetIfNotExists_DoesnotSet()
        {
            var cm = new TestMemoryCacheManager();
            cm.Flush();
            var key1 = "key1";
            var val = "value1";

            cm.TempCache[key1] = val;
            cm.SetIfNotExists(key1, "value2");
            cm.Get<string>(key1).ShouldBe(val);
        }

        [Test]
        public void CacheManger_SetIfNotExists_Set()
        {
            var cm = new TestMemoryCacheManager();
            cm.Flush();
            var key1 = "key1";
            var val = "value1";

            cm.SetIfNotExists(key1, val);
            cm.Get<string>(key1).ShouldBe(val);
        }
    }
}
