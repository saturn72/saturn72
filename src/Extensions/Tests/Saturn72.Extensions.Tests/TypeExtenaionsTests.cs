#region

using System;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class TypeExtenaionsTests
    {
        [Test]
        public void GetAttributeValue_ReturnAttributesValue()
        {
            var value = typeof (MyClass1).GetAttributeValue((MyClassTestAttribute x) => x.Value);
            value.ShouldEqual("value");
        }

        [Test]
        public void GetAttributeValue_ReturnsDefaultWhenAttributesNotExists()
        {
            var value = typeof (MyClass2).GetAttributeValue((MyClassTestAttribute x) => x.Value);
            value.ShouldEqual(default(string));
        }

        [MyClassTest("value")]
        public class MyClass1
        {
        }

        public class MyClass2
        {
        }

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class MyClassTestAttribute : Attribute
        {
            public MyClassTestAttribute(string value)
            {
                Value = value;
            }

            public string Value { get; private set; }
        }
    }
}