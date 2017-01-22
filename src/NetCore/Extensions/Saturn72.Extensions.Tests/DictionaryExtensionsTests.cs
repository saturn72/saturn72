
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Extensions.Tests
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void GetOrDefault_ReturnsDefault()
        {
            var source = new Dictionary<string, string>();
            var actual = source.GetValueOrDefault("TTT");
            actual.ShouldEqual(default(string));

            actual = source.GetValueOrDefault("TTT", "d");
            actual.ShouldEqual("d");
        }

        [Fact]
        public void GetOrDefault_ReturnsValue()
        {
            var source = new Dictionary<string, string>{{"TTT", "!"}};
            var actual = source.GetValueOrDefault("TTT");
            actual.ShouldEqual("!");

            actual = source.GetValueOrDefault("TTT", "d");
            actual.ShouldEqual("!");
        }

        [Fact]
        public void GetValueOrSet_ReturnsExistValue()
        {
            const string value1 = "value1";

            var source = new Dictionary<string, string>
            {
                {"key1", value1},
                {"key2", "value2"},
            };

            var actual = source.GetValueOrSet("key1", "newValue1");
            actual.ShouldEqual(value1);
        }

        [Fact]
        public void GetValueOrSet_ReturnsSetValue()
        {
            var source = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"},
            };

            const string value3 = "value3";
            var actual = source.GetValueOrSet("key3", value3);
            actual.ShouldEqual(value3);
        }

        [Fact]
        public void GetOrDefault_ThrowOnNull()
        {
            Assert.Throws<NullReferenceException>(
                () => ((Dictionary<object, object>)null).GetValueOrDefault(new object()));
        }


        [Fact]
        public void GetOrDefault_ReferenceType_ReturndsDefaultOnNonExistsKey()
        {
            var refKey1 = new TestKeyObject { Key = "key" };
            var refValue1 = new TestValueObject { Value = "key" };
            var valueTypeSource = new Dictionary<TestKeyObject, TestValueObject>
        {
            { refKey1, refValue1}
            };

            Assert.AreEqual(default(TestValueObject), valueTypeSource.GetValueOrDefault(new TestKeyObject()));
        }

        [Fact]
        public void GetOrDefault_ReferenceType_ReturnsValueOnExistsKey()
        {
            var refKey1 = new TestKeyObject { Key = "key" };
            var refValue1 = new TestValueObject { Value = "key" };
            var valueTypeSource = new Dictionary<TestKeyObject, TestValueObject>
        {
            { refKey1, refValue1}
        };

            Assert.AreEqual(refValue1, valueTypeSource.GetValueOrDefault(refKey1));
        }

        public class TestKeyObject
        {
            public string Key { get; set; }
        }
        public class TestValueObject
        {
            public string Value { get; internal set; }
        }

        [Fact]
        public void GetOrDefault_ValueType_ReturndsDefaultOnNonExistsKey()
        {
            var valueTypeSource = new Dictionary<string, int>
        {
            { "1", 1 },
            {"2", 2 }
            };
            Assert.AreEqual(0, valueTypeSource.GetValueOrDefault("key"));
        }

        [Fact]
        public void GetOrDefault_ValueType_ReturnsValueOnExistsKey()
        {
            var valueTypeSource = new Dictionary<string, int>
        {
            { "1", 1 },
            {"2", 2 }
            };
            Assert.AreEqual(2, valueTypeSource.GetValueOrDefault("2"));
        }
    }
}
