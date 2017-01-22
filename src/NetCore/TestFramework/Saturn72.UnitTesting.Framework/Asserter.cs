using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Saturn72.UnitTesting.Framework
{
    public static class Asserter
    {
        public static void ShouldNotContain(string source, string containedString)
        {
            var message = string.Format("The collection string {0} should not contain any instance of the string {1}",
                source,
                containedString);
            ShouldNotContain(source, containedString, message);
        }

        public static void ShouldNotContain(string source, string containedString, string message)
        {
            Assert.False(source.Contains(containedString), message);
        }

        public static void ShouldContain<T>(IEnumerable<T> collection, T contained, int occurrencesCount)
        {
            ShouldContain(collection, contained, occurrencesCount,
                string.Format("The collection does not contains {0} instances of {1}",                     occurrencesCount, contained));
        }

        public static void ShouldContain<T>(IEnumerable<T> collection, T contained, int occurrencesCount,
            string message)
        {
            ShouldContainRange(collection, contained, occurrencesCount, occurrencesCount, message);
        }

        public static void ShouldContainMaximum<T>(IEnumerable<T> collection, T contained, int maxOccurances)
        {
            var message = string.Format("Expected maximum {0} occurances of the item {1} in collection {2}",
                maxOccurances, contained, collection);
            ShouldContainMaximum(collection, contained, maxOccurances, message);
        }

        public static void ShouldContainMaximum<T>(IEnumerable<T> collection, T contained, int maxOccurances,
            string message)
        {
            ShouldContainRange(collection, contained, 1, maxOccurances, message);
        }

        public static void ShouldContainMinimum<T>(IEnumerable<T> collection, T contained, int minOccurances)
        {
            var message = string.Format("Expected minimum {0} occurances of the string {1} in collection string {2}",
                minOccurances, contained, collection);
            ShouldContainMinimum(collection, contained, minOccurances, message);
        }

        public static void ShouldContainMinimum<T>(IEnumerable<T> collection, T contained, int minOccurances,
            string message)
        {
            ShouldContainRange(collection, contained, minOccurances, int.MaxValue, message);
        }


        public static void ShouldContainRange<T>(IEnumerable<T> collection, T contained, int minOccurances,
            int maxOccurances)
        {
            var message = string.Format(
                "Expected minimum occurances of {0} and maximum occurances of {1} of the string {2} in collection string {3}",
                minOccurances, maxOccurances, contained, collection);

            ShouldContainRange(collection, contained, minOccurances, maxOccurances, message);
        }


        public static void ShouldContainRange<T>(IEnumerable<T> collection, T contained, int minOccurances,
            int maxOccurances, string message)
        {
            var totalOccurances = collection.Count(itm=>itm.Equals(contained));
            Assert.True(totalOccurances >= minOccurances && totalOccurances <= maxOccurances, message);
        }

        public static void ShouldContain<T>(IEnumerable<T> collection, T contained)
        {
            ShouldContain(collection, contained,
                string.Format("The item {0} does not contained the collection", contained));
        }

        public static void ShouldContain<T>(IEnumerable<T> collection, T contained, string message)
        {
            ShouldContainRange(collection, contained, 1, Int32.MaxValue, message);
        }

        public static void Greater(IComparable actual, IComparable expected)
        {
            Greater(actual, expected, string.Format(
                "Expected number greater than, but was smaller or equal. Actual: {0}, Expected greather than: {1}",
                actual, expected));
        }

        public static void Greater(IComparable actual, IComparable expected, string message)
        {
            var res = actual.CompareTo(expected);
            Assert.True(res>0, message);
        }

        public static void GreaterOrEqual(IComparable actual, IComparable expected)
        {
            GreaterOrEqual(actual, expected, string.Format(
                "Expected greater or equal to, but was smaller. Actual: {0}, Expected greather than: {1}",
                actual, expected));
        }

        public static void GreaterOrEqual(IComparable actual, IComparable expected, string message)
        {
            var res = actual.CompareTo(expected);
            Assert.True(res >= 0, message);
        }

        public static void SmallerOrEqual(IComparable actual, IComparable expected)
        {
            SmallerOrEqual(actual, expected, string.Format(
                "Expected number smaller or equal to, but was greater. Actual: {0}, Expected greather than: {1}",
                actual, expected));
        }

        public static void SmallerOrEqual(IComparable actual, IComparable expected, string message)
        {
            var res = actual.CompareTo(expected);
            Assert.True(res <= 0, message);
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
            var res = actual.CompareTo(expected);
            Assert.True(res < 0, message);
        }

        public static void True(bool condition)
        {
            Assert.True(condition);
        }

        public static void True(bool condition, string msg)
        {
            Assert.True(condition, msg);
        }


        public static void NotNull(object o)
        {
            Assert.NotNull(o);
        }

        public static void Null(object o)
        {
            Assert.Null(o);
        }


        public static void AreEqual(object a, object b, string message)
        {
            //xunit syntax
            Assert.True(a.Equals(b), message);
        }

        public static void AreEqual(object a, object b)
        {
            Assert.Equal(a, b);
        }

        public static void AreNotEqual(object a, object b, string message)
        {
            //xunit syntax
            Assert.True(a != b, message);
        }

        public static void AreNotEqual(object a, object b)
        {
            Assert.NotEqual(a, b);
        }


      

        public static void False(bool condition)
        {
            Assert.False(condition);
        }

        public static void False(bool condition, string msg)
        {
            Assert.False(condition, msg);
        }

        public static Exception Throws<TException>(TException exception, Action testCode) where TException:Exception
        {
            return Throws(typeof(TException), testCode);
        }

        public static Exception Throws(Type exceptionType, Action testCode) 
        {
            return Assert.Throws(exceptionType, testCode);
        }

        public static void Fail(string message)
        {
            True(false, message);
        }

        public static void AreSame(object expected, object actual)
        {
            Assert.Same(expected, actual);
        }

        public static void AreNotSame(object expected, object actual)
        {
            Assert.NotSame(expected, actual);
        }

    }
}