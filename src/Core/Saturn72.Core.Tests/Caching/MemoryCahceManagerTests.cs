using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Tests.Caching
{
    public class MemoryCahceManagerTests
    {
        [Test]
        public void CacheManger_Get_ItemNotCached_Throws()
        {
            var cm = new MemoryCacheManager();
            typeof(NullReferenceException).ShouldBeThrownBy(() => cm.Get<int>("not_cached"));
        }

        [Test]
        public void CacheManger_Get_ItemCached_ReturnsValue()
        {
            var cm = new TestMemoryCacheManager();
            string key = "cached",
                value = "valeu";
            cm.TempCache.Set(key, value, DateTimeOffset.MaxValue);
            cm.Get<string>(key).ShouldEqual(value);
        }

        [Test]
        public void CacheManger_Set_NewItem()
        {
            var cm = new TestMemoryCacheManager();
            string key = "cached",
                value = "valeu";
            cm.Set(key, value, 100);

            cm.TempCache[key].ShouldEqual(value);
        }

        [Test]
        public void CacheManger_Set_NewItem_CachingTime()
        {
            var cm = new TestMemoryCacheManager();
            string key = "cached",
                value = "valeu";
            cm.Set(key, value, 1);
            Thread.Sleep(60100);
            typeof(NullReferenceException).ShouldBeThrownBy(() => cm.Get<int>(key));
        }

        [Test]
        public void CacheManger_Set_ItemCached()
        {
            var cm = new TestMemoryCacheManager();
            string key = "cached",
                value = "valeu";
            cm.Set(key, value, 100);

            var value2 = "val2";
            cm.Set(key, value2, 100);

            (cm.TempCache[key] as string).ShouldEqual(value2);
        }

        [Test]
        public void CacheManger_Clear_OnEmptyCache_DoesNothing()
        {
            var cm = new TestMemoryCacheManager();
            cm.Clear();
            cm.TempCache.GetCount().ShouldEqual(0);
        }

        [Test]
        public void CacheManger_Clear_RemovesAll()
        {
            var cm = new TestMemoryCacheManager();
            cm.TempCache["key1"] = "value1";
            cm.TempCache["key2"] = "value2";
            cm.TempCache["key3"] = "value3";
            cm.Clear();

            cm.TempCache.GetCount().ShouldEqual(0);
        }


        [Test]
        public void CacheManger_Clear_ClearsCache()
        {
            var cm = new TestMemoryCacheManager();
            //On empty cache
            cm.Clear();
            cm.TempCache.GetCount().ShouldEqual(0);

            //on flled cache
            var key1 = "key1";
            cm.TempCache[key1] = "value1";
            cm.Clear();
            cm.TempCache.GetCount().ShouldEqual(0);
        }

        [Test]
        public void CacheManger_Remove_ClearsCache()
        {
            var cm = new TestMemoryCacheManager();
            cm.Flush();
            //On not exists cache key
            cm.Remove("123");
            cm.TempCache.GetCount().ShouldEqual(0);

            //on flled cache
            var key1 = "key1";
            cm.TempCache[key1] = "value1";
            cm.Remove(key1);
            cm.TempCache.GetCount().ShouldEqual(0);
        }

        [Test]
        public void CacheManger_Keys_GetAll()
        {
            var cm = new TestMemoryCacheManager();
            cm.Flush();
            //On not exists cache key
            cm.Keys.Count().ShouldEqual(0);

            var key1 = "key1";
            cm.TempCache[key1] = "value1";
            var keys = cm.Keys;
            keys.Count().ShouldEqual(1);
            keys.ElementAt(0).ShouldEqual(key1);
        }
    }
}