#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.UT
{
    public class EnumerableExtensionTests
    {
        [Test]
        public void ForEachItem_ThrowsException()
        {
            var i = 0;
            Assert.Throws<NullReferenceException>(() => ((List<object>) null).ForEachItem(c => i++));
        }

        [Test]
        public void ForEachItem_IteratesOnCollection()
        {
            var i = 0;
            new[] {1, 2, 3}.ForEachItem(c => i++);
            Assert.AreEqual(3, i);
        }

        [Test]
        public void IsEmptyOrNull_returnsTrueCases()
        {
            new List<string>().IsEmptyOrNull().ShouldBeTrue();

            ((IEnumerable<string>) null).IsEmptyOrNull().ShouldBeTrue();
            "".IsEmptyOrNull().ShouldBeTrue();
        }

        [Test]
        public void IsEmptyOrNull_ReturnsFalseCases()
        {
            "Test".IsEmptyOrNull().ShouldBeFalse();
        }
        [Test]
        public void Random_PicksItems()
        {
            var action = new Action(() =>
            {
                var source = new[] { 1, 2 };
                var actualValue = source.Random();
                Assert.IsTrue(1 == actualValue || 2 == actualValue);

            });

            for (int i = 0; i < 1000; i++)
            {
                action();
            }
        }
    }
}