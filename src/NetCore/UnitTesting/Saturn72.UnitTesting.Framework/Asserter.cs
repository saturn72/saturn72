using System;

namespace Saturn72.UnitTesting.Framework
{
    public static class Asserter
    {
        public static void AreEqual(object a, object b, string message)
        {
            Assert.AreEqual(a, b, message);
        }

        public static void AreEqual(object a, object b)
        {
            Assert.AreEqual(a, b);
        }
        public static void ShouldNotContain(this string source, string containedString)
        {
            var message = string.Format("The source string {0} should not contain any instance of the string {1}",
                source,
                containedString);
            ShouldNotContain(source, containedString, message);
        }

        public static void ShouldNotContain(this string source, string containedString, string message)
        {
            Assert.False(source.Contains(containedString), message);
        }

        public static void ShouldContain(this string source, string containedString, int occurrencesCount)
        {
            ShouldContain(source, containedString, occurrencesCount,
                string.Format("The string {0} does not contains {1} instances of the string {2}", source,
                    occurrencesCount, containedString));
        }

        public static void ShouldContain(this string source, string containedString, int occurrencesCount,
            string message)
        {
            ShouldContainRange(source, containedString, occurrencesCount, occurrencesCount, message);
        }

        public static void ShouldContainMaximum(this string source, string containedString, int maxOccurances)
        {
            var message = string.Format("Expected maximum {0} occurances of the string {1} in source string {2}",
                maxOccurances, containedString, source);
            ShouldContainMaximum(source, containedString, maxOccurances, message);
        }

        public static void ShouldContainMaximum(this string source, string containedString, int maxOccurances,
            string message)
        {
            ShouldContainRange(source, containedString, 1, maxOccurances, message);
        }

        public static void ShouldContainMinimum(this string source, string containedString, int minOccurances)
        {
            var message = string.Format("Expected minimum {0} occurances of the string {1} in source string {2}",
                minOccurances, containedString, source);
            ShouldContainMinimum(source, containedString, minOccurances, message);
        }

        public static void ShouldContainMinimum(this string source, string containedString, int minOccurances,
            string message)
        {
            ShouldContainRange(source, containedString, minOccurances, int.MaxValue, message);
        }


        public static void ShouldContainRange(this string source, string containedString, int minOccurances,
            int maxOccurances)
        {
            var message = string.Format(
                "Expected minimum occurances of {0} and maximum occurances of {1} of the string {2} in source string {3}",
                minOccurances, maxOccurances, containedString, source);

            ShouldContainRange(source, containedString, minOccurances, maxOccurances, message);
        }


        public static void ShouldContainRange(this string source, string containedString, int minOccurances,
            int maxOccurances, string message)
        {
            var totalOccurances = source.Split(new[] { containedString }, StringSplitOptions.RemoveEmptyEntries).Length -
                                  1;
            Assert.True(totalOccurances >= minOccurances && totalOccurances <= maxOccurances, message);
        }

        public static void ShouldContain(this string source, string containedString)
        {
            ShouldContain(source, containedString,
                string.Format("The string {0} does not contains the string {1}", source, containedString));
        }

        public static void ShouldContain(this string source, string containedString, string message)
        {
            ShouldContainRange(source, containedString, 1, Int32.MaxValue, message);
        }

        public static void Greater(IComparable actual, IComparable expected)
        {
            Greater(actual, expected, string.Format(
                "Expected number greater than, but was smaller or equal. Actual: {0}, Expected greather than: {1}",
                actual, expected));
        }

        public static void Greater(IComparable actual, IComparable expected, string message)
        {
            Assert.Greater(actual, expected, message);
        }

        public static void GreaterOrEqual(IComparable actual, IComparable expected)
        {
            GreaterOrEqual(actual, expected, string.Format(
                "Expected greater or equal to, but was smaller. Actual: {0}, Expected greather than: {1}",
                actual, expected));
        }

        public static void GreaterOrEqual(IComparable actual, IComparable expected, string message)
        {
            Assert.GreaterOrEqual(actual, expected, message);
        }

        public static void SmallerOrEqual(IComparable actual, IComparable expected)
        {
            SmallerOrEqual(actual, expected, string.Format(
                "Expected number smaller or equal to, but was greater. Actual: {0}, Expected greather than: {1}",
                actual, expected));
        }

        public static void SmallerOrEqual(IComparable actual, IComparable expected, string message)
        {
            Assert.LessOrEqual(actual, expected, message);
        }

        public static void Smaller(IComparable actual, IComparable expected)
        {
            Smaller(actual, expected,
                string.Format(
                    "Expected number smaller than, but was greater or equal. Actual: {0}, Expected greather than: {1}",
                    actual, expected));
        }

        public static void Smaller(IComparable actual, IComparable expected, string message)
        {
            Assert.Less(actual, expected, message);
        }

        public static void True(bool condition)
        {
            Assert.True(condition);
        }

        public static void True(bool condition, string msg)
        {
            Assert.True(condition, msg);
        }
    }
}