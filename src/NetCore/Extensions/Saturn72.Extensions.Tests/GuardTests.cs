#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class GuardTests
    {
        [Fact]
        public void Guard_GreaterThan()
        {
            //throws on not greater than
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(()=> Guard.GreaterThan(1,1));
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => Guard.GreaterOrEqualTo(1, 10));

            Guard.GreaterThan(1,0);
        }

        [Fact]
        public void Guard_GreaterOrEqualsTo()
        {
            //throws on not greater than
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => Guard.GreaterOrEqualTo(1, 10));

            Guard.GreaterOrEqualTo(1, 1);
            Guard.GreaterOrEqualTo(1, 0);
        }
        [Fact]
        public void Guard_SmallerThan()
        {
            //throws on not greater than
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => Guard.SmallerThan(1, 1));
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => Guard.SmallerThan(1, 0));

            Guard.SmallerThan(1, 10);
        }

        [Fact]
        public void Guard_SmallerOrEqualsTo()
        {
            //throws on not greater than
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => Guard.SmallerOrEqualTo(10, 1));

            Guard.SmallerOrEqualTo(1, 1);
            Guard.SmallerOrEqualTo(1, 10);
        }

        [Fact]
        public void ContainsKey_KeyNotExists()
        {
            var dictionary = new Dictionary<int, int> {{1, 1}, {2, 2}, {3, 4}};

            Assert.Throws<KeyNotFoundException>(() =>
                Guard.ContainsKey(dictionary, 9));
        }

        [Fact]
        public void ContainsKey_KeyExists()
        {
            var dictionary = new Dictionary<int, int> {{1, 1}, {2, 2}, {3, 4}};
            Assert.DoesNotThrow(() => Guard.ContainsKey(dictionary, 1));
        }

        [Fact]
        public void IsUrl_ThrowsException()
        {
            Assert.Throws<FormatException>(() => Guard.IsUrl("VVV"));
        }

        [Fact]
        public void IsUrl_DoesNotThrowsException()
        {
            Assert.DoesNotThrow(() => Guard.IsUrl("http://www.ttt.com"));
        }

        [Fact]
        public void MustFollows_DoesNotTriggerAction()
        {
            var str = "test";
            Guard.MustFollow(str.Length == 4, () => str = str.ToUpper());
            "test".ShouldEqual(str);
        }

        [Fact]
        public void MustFollows_TriggerAction()
        {
            var str = "test";
            Guard.MustFollow(() => str.Length == 0, () => str = str.ToUpper());
            "TEST".ShouldEqual(str);
        }

        [Fact]
        public void NotEmpty_TriggersAction()
        {
            var x = 0;
            Guard.NotEmpty(new List<string>(), () => x++);
            1.ShouldEqual(x);
        }

        [Fact]
        public void HasValue_DoesNotTriggersAction()
        {
            var x = 0;
            Guard.HasValue("test", () => x++);
            0.ShouldEqual(x);
        }

        [Fact]
        public void HasValue_TriggersAction()
        {
            var x = 0;
            Guard.HasValue("", () => x++);
            1.ShouldEqual(x);
        }

        [Fact]
        public void HasValue_ThrowsExceptionOnEmptyString()
        {
            typeof(ArgumentNullException).ShouldBeThrownBy(
                () => Guard.HasValue("", () => { throw new ArgumentNullException(); }));
        }

        [Fact]
        public void NotNull_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => Guard.NotNull((object) null));
        }

        [Fact]
        public void NotNull_ThrowsNullReferenceExceptionWithMessage()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => Guard.NotNull((object) null, "message"),
                "message");
        }

        [Fact]
        public void NotNull_TriggerAction()
        {
            var x = 0;
            Guard.NotNull((object) null, () => x++);
            1.ShouldEqual(x);
        }
    }
}