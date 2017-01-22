using System;
using System.Collections.Generic;
using Xunit;

namespace Saturn72.UnitTesting.Framework.Tests
{
    public class TestExtensionsTests
    {

        [Fact]
        public void ShouldContainAny()
        {
            "this is string".ShouldContainAny("s1222");
            "this is string".ShouldContainAny("str", "message");
        }


        [Fact]
        public void ShouldContainAll()
        {
            "this is string".ShouldContainAll("sith");
            "this is string".ShouldContainAll("str", "message");
        }


        [Fact]
        public void ShouldContain()
        {
            "this is string".ShouldContainAny("str");
            "this is string".ShouldContain("str", "message");
        }

       //[Fact]
       // public void ShouldContain_Instances()
       // {
       //     "this is string".ShouldContain("str", 1);
       //     "this is string".ShouldContain("str", 1, "message");
       // }


       //[Fact]
       // public void ShouldContainMax()
       // {
       //     "this is string".ShouldContainMaximum("str", 2);
       //     "this is string".ShouldContainMaximum("str", 2, "message");
       // }

       //[Fact]
       // public void ShouldContainMin()
       // {
       //     "this is string".ShouldContainMinimum("is", 2);
       //     "this is string".ShouldContainMinimum("is", 2, "message");
       // }

       //[Fact]
       // public void ShouldContainRange()
       // {
       //     var message = "message";
       //     "this is string".ShouldContainRange("is", 2, 6);
       //     "this is string".ShouldContainRange("is", 2, 6, message);

       //     "this is string that is beautiful".ShouldContainRange("is", 3, 6);
       //     "this is string that is beautiful".ShouldContainRange("is", 3, 6, message);
       //     "this is string that is beautiful is is ".ShouldContainRange("is", 4, 6);
       //     "this is string that is beautiful is is ".ShouldContainRange("is", 4, 6, message);
       //     "is is this is string that is beautiful is is ".ShouldContainRange("is", 5, 6);
       //     "is is this is string that is beautiful is is ".ShouldContainRange("is", 5, 6, message);
       // }

       //[Fact]
       // public void ShouldNotContain()
       // {
       //     "this is string".ShouldNotContain("strsss");
       // }

       [Fact]
        public void True()
        {
            true.True();
        }

       [Fact]
        public void True_WithMessage()
        {
            true.True("message");
        }

       [Fact]
        public void ShouldNotBeNull()
        {
            new TestClass().ShouldNotBeNull();
            "rrr".ShouldNotBeNull();
        }

       [Fact]
        public void ShouldNotBeNull_WithMessage()
        {
            new TestClass().ShouldNotBeNull("message");
            "rrr".ShouldNotBeNull("message");
        }

       [Fact]
        public void ShouldNotEqual()
        {
            new TestClass().ShouldNotEqual(new TestClass());
            1.ShouldNotEqual(0);
            "ttt".ShouldNotEqual("rrr");
        }

       [Fact]
        public void ShouldNotEqual_WithMessage()
        {
            "ttt".ShouldNotEqual("rrr", "message");
        }

       [Fact]
        public void ShouldEqual()
        {
            var instance = new TestClass();
            instance.ShouldEqual(instance);
            1.ShouldEqual(1);
            "ttt".ShouldEqual("ttt");
        }

        /// <summary>
        ///     Asserts that two objects are equal.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
       [Fact]
        public void ShouldEqual_WithMessage()
        {
            "ttt".ShouldEqual("ttt", "message");
        }

       [Fact]
        public void ShouldBeThrownBy()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => { throw new NullReferenceException(); }
                );
        }

       [Fact]
        public void ShouldBeThrownBy_WithMessage()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => { throw new NullReferenceException(); }
                , "Object reference not set to an instance of an object.");
        }

       [Fact]
        public void ShouldBe()
        {
            "ttt".ShouldBe<string>();
        }

       [Fact]
        public void ShouldBeType()
        {
            "ttt".GetType().ShouldBeType<string>();
        }

       [Fact]
        public void ShouldBeNull()
        {
            ((object)null).ShouldBeNull();
        }

       [Fact]
        public void ShouldBeTheSameAs()
        {
            var actual = new TestClass();
            var expected = actual;
            actual.ShouldBeTheSameAs(expected);
        }

       [Fact]
        public void ShouldBeNotBeTheSameAs()
        {
            var actual = new TestClass();
            var expected = new TestClass();
            actual.ShouldBeNotBeTheSameAs(expected);
        }

       [Fact]
        public void ShouldBeTrue()
        {
            true.ShouldBeTrue();
        }

       [Fact]
        public void ShouldBeTrue_WitrhMessage()
        {
            true.ShouldBeTrue("message");
        }

       [Fact]
        public void ShouldBeFalse()
        {
            false.ShouldBeFalse();
        }

       [Fact]
        public void AssertSameStringAs()
        {
            "ttt".AssertSameStringAs("ttt");
        }

       [Fact]
        public void ShouldContainInstance()
        {
            var instance = new TestClass();
            var source = new List<TestClass> { new TestClass(), instance, new TestClass() };

            source.ShouldContain(instance);
        }

       [Fact]
        public void ShouldContainType()
        {
            var instance = new TestClass();
            var source = new List<TestClass> { new TestClass(), instance, new TestClass() };

            source.ShouldContainType(instance.GetType());
        }

       [Fact]
        public void ShouldBeEmpty()
        {
            new List<TestClass>().ShouldBeEmpty();
        }

       [Fact]
        public void ShouldCount()
        {
            var source = new List<TestClass> { new TestClass(), new TestClass(), new TestClass() };
            source.ShouldCount(3);
        }

       [Fact]
        public void PropertyValuesAreEquals_AllProperties()
        {
            var instance1 = new TestClass
            {
                DecimalValue = 1,
                StringValue = "T"
            };

            var instance2 = new TestClass
            {
                DecimalValue = 1,
                StringValue = "T"
            };
            instance1.PropertyValuesAreEquals(instance2);
        }

       [Fact]
        public void PropertyValuesAreEquals_ExcludeProperties()
        {
            var instance1 = new TestClass
            {
                DecimalValue = 1,
                StringValue = "T",
                ExcludedProp = "111"
            };

            var instance2 = new TestClass
            {
                DecimalValue = 1,
                StringValue = "T",
                ExcludedProp = "eeecd"
            };
            instance1.PropertyValuesAreEquals(instance2, new[] { "ExcludedProp" });
        }
    }

    public class TestClass
    {
        public int DecimalValue { get; set; }
        public string StringValue { get; set; }
        public string ExcludedProp { get; set; }
    }
}