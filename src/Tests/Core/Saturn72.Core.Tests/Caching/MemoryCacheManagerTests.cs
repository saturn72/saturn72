#region

using System;
using System.Threading;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Tests.Caching
{
    public class MemoryCacheManagerTests
    {
        [Test]
        public void MemoryCacheManagerTests_Get()
        {
            var cm = new MemoryCacheManagerTestObject();
            cm.Get<object>("rrr").ShouldBeNull();

            var value = "Dummy";
            cm.InsertToCache("rrr", value);
            cm.Get<string>("rrr").ShouldEqual(value);
        }

        [Test]
        public void MemoryCacheManagerTests_Set()
        {
            var cm = new MemoryCacheManagerTestObject();
            var key = "key";
            var value1 = "value1";

            cm.IsInCache(key).ShouldBeFalse();

            cm.Set(key, value1, 60);
            cm.IsInCache(key).ShouldBeTrue();
        }
        [Test]
        public void MemoryCacheManagerTests_SetThenGet()
        {
            var cm = new MemoryCacheManager();
            var key = "key";
            var value1 = "value1";
            var value2 = "value2";

            cm.Set(key, value1, 60);
            cm.Get<string>(key).ShouldEqual(value1);

            cm.Set(key, value2, 60);
            cm.Get<string>(key).ShouldEqual(value2);
        }

        [Test]
        public void MemoryCacheManagerTests_IsSet()
        {
            var cm = new MemoryCacheManagerTestObject();
            
            var key = "rrr";
            var value = "Dummy";
            cm.IsSet(key).ShouldBeFalse();

            cm.InsertToCache(key, value);
            cm.IsSet(key).ShouldBeTrue();
        }

        [Test]
        public void MemoryCacheManagerTests_Remove()
        {
            var cm = new MemoryCacheManagerTestObject();

            var key = "key";
            var value = "value";

            cm.InsertToCache(key, value);
            cm.IsInCache(key).ShouldBeTrue();
            cm.Remove(key);
            cm.IsInCache(key).ShouldBeFalse();
        }

        [Test]
        public void MemoryCacheManagerTests_RemoveByPattern()
        {
            var cm = new MemoryCacheManagerTestObject();

            var key = "key";
            var value = "value";

            cm.InsertToCache(key, value);
            cm.IsInCache(key).ShouldBeTrue();
            cm.RemoveByPattern(key.Substring(0,key.Length-2) + ".*");
            cm.IsInCache(key).ShouldBeFalse();
        }


        [Test]
        public void MemoryCacheManagerTests_Clear()
        {
            var cm = new MemoryCacheManagerTestObject();

            var key1 = "key";
            var value1 = "value";

            var key2 = "key";
            var value2 = "value";


            cm.InsertToCache(key1, value1);
            cm.InsertToCache(key2, value2);
            cm.IsInCache(key1).ShouldBeTrue();
            cm.IsInCache(key2).ShouldBeTrue();

            cm.Clear();

            cm.IsInCache(key1).ShouldBeFalse();
            cm.IsInCache(key2).ShouldBeFalse();
        }

        internal class MemoryCacheManagerTestObject : MemoryCacheManager
        {
            internal void InsertToCache(string key, object value)
            {
                Cache.Add(key, value, DateTime.Now + TimeSpan.FromMinutes(60));
            }

            internal bool IsInCache(string key)
            {
                return Cache.Contains(key);
            }
        }
    }
}