using System;

namespace Saturn72.UnitTesting.Framework
{
    public static partial class TestExtensions
    {
        public static void ShouldBeGreaterThan(this IComparable actual, IComparable num)
        {
            Asserter.Greater(actual, num);
        }

        public static void ShouldBeGreaterOrEqualTo(this IComparable actual, IComparable num)
        {
            Asserter.GreaterOrEqual(actual, num);
        }

        public static void ShouldBeSmallerOrEqualTo(this IComparable actual, IComparable num)
        {
            Asserter.SmallerOrEqual(actual, num);
        }

        public static void ShouldBeSmallerThan(this IComparable actual, IComparable num)
        {
            Asserter.Smaller(actual, num);
        }

    }
}