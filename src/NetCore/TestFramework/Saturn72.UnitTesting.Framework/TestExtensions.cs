using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Saturn72.UnitTesting.Framework
{
    public static partial class TestExtensions
    {
        public static void True(this bool b)
        {
            Asserter.True(b);
        }

        public static void True(this bool b, string message)
        {
            Asserter.True(b, message);
        }

        public static void ShouldNotBeNull<T>(this T obj)
        {
            Asserter.NotNull(obj);
        }

        public static void ShouldNotBeNull<T>(this T obj, string message)
        {
            var condition = obj != null;
            Asserter.True(condition, message);
        }

        public static void ShouldNotEqual<T>(this T actual, object expected)
        {
            Asserter.AreNotEqual(expected, actual);
        }

        public static void ShouldNotEqual<T>(this T actual, object expected, string message)
        {
            var result = expected.Equals(actual);
            Asserter.False(result, message);
        }

        public static void ShouldEqual<T>(this T actual, object expected)
        {
            Asserter.AreEqual(expected, actual);
        }

        /// <summary>
        ///     Asserts that two objects are equal.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="message"></param>
        public static void ShouldEqual(this object actual, object expected, string message)
        {
            Asserter.AreEqual(expected, actual);
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, Action testCode)
        {
            return Asserter.Throws(exceptionType, testCode);
        }

        public static Exception ShouldBeThrownBy(this Type exceptionType, Action testCode, string exceptionMEssage)
        {
            var t = new Task(() => { });
            var exception = Asserter.Throws(exceptionType, () => testCode());
            Asserter.AreEqual(exceptionMEssage, exception.Message);

            return exception;
        }

        public static void ShouldBe<T>(this object actual)
        {
            Asserter.AreEqual(typeof(T), actual.GetType());
        }

        public static void ShouldBeType<T>(this Type actual)
        {
            Asserter.AreEqual(typeof(T), actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Asserter.Null(actual);
        }

        public static void ShouldBeTheSameAs(this object actual, object expected)
        {
            Asserter.AreSame(expected, actual);
        }

        public static void ShouldBeNotBeTheSameAs(this object actual, object expected)
        {
            Asserter.AreNotSame(expected, actual);
        }

        public static void ShouldBeTrue(this bool source)
        {
            True(source);
        }

        public static void ShouldBeTrue(this bool source, string message)
        {
            True(source, message);
        }

        public static void ShouldBeFalse(this bool source)
        {
            Asserter.False(source);
        }


        /// <summary>
        ///     Compares the two strings (case-insensitive).
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static void AssertSameStringAs(this string actual, string expected)
        {
            if (!string.Equals(actual, expected, StringComparison.CurrentCultureIgnoreCase))
            {
                var message = string.Format("Expected {0} but was {1}", expected, actual);
                throw new ArgumentException(message);
            }
        }


        public static void ShouldContain(this string source, string subString)
        {
            Asserter.True(source.Contains(subString));
        }
        public static void ShouldContain(this string source, string subString, string message)
        {
            Asserter.True(source.Contains(subString), message);
        }

        public static void ShouldContainAny<T>(this IEnumerable<T> collection, IEnumerable<T> instanceRange)
        {
            Asserter.True(instanceRange.FirstOrDefault(x => collection.Contains(x)) != null);
        }

        public static void ShouldContainAny<T>(this IEnumerable<T> superset, IEnumerable<T> subset, string message)
        {
            var condition = subset.FirstOrDefault(x => superset.Contains(x)) != null;
            Asserter.True(condition, message);
        }

        public static void ShouldContainAll<T>(this IEnumerable<T> superset, IEnumerable<T> subset)
        {
            var condition = subset.All(i => superset.Contains(i));
            Asserter.True(condition);
        }

        public static void ShouldContainAll<T>(this IEnumerable<T> superset, IEnumerable<T> subset, string message)
        {
            var condition = subset.All(i => superset.Contains(i));
            Asserter.True(condition, message);
        }

        public static void ShouldContain<T>(this IEnumerable<T> collection, T instance)
        {
            Asserter.ShouldContain(collection, instance);
        }

        public static void ShouldContainType(this IEnumerable collection, Type expectedType)
        {
            var result = collection.Cast<object>().Any(s => s.GetType() == expectedType);
            Asserter.True(result);
        }

        public static void ShouldBeEmpty(this IEnumerable collection)
        {
            Asserter.AreEqual(collection.Cast<object>().Count(), 0);
        }

        public static void ShouldCount(this IEnumerable source, int expected)
        {
            source.Cast<object>().ToArray().Length.ShouldEqual(expected);
        }

        public static void PropertyValuesAreEquals<TType>(this TType actual, TType expected)

        {
            PropertyValuesAreEquals(actual, expected, new string[] {});
        }

        public static void PropertyValuesAreEquals<TType>(this TType actual, TType expected, string[] execludeProperties)
        {
            var properties = expected.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (execludeProperties.Contains(property.Name, StringComparer.CurrentCultureIgnoreCase))
                    continue;

                var expectedValue = property.GetValue(expected, null);
                var actualValue = property.GetValue(actual, null);

                if (actualValue is IDictionary)
                {
                    AssertDictionariesAreEquals(property, (IDictionary) actualValue, (IDictionary) expectedValue);
                    continue;
                }
                if (actualValue is IList)
                {
                    AssertListsAreEquals(property, (IList) actualValue, (IList) expectedValue);
                    continue;
                }

                if (!Equals(expectedValue, actualValue))
                    Asserter.Fail(
                        $"Property {property.DeclaringType.Name}.{property.Name} does not match. Expected: {expectedValue} but was: {actualValue}");
            }
        }

        private static void AssertDictionariesAreEquals(PropertyInfo property, IDictionary actual, IDictionary expected)
        {
            if (actual.Count != expected.Count)
                Asserter.Fail(
                    $"Property {property.PropertyType.Name}.{property.Name} does not match. Expected IDictionary containing {expected.Count} elements but was IDictionary containing {actual.Count} elements");

            for (var i = 0; i < actual.Count; i++)
                if (!Equals(actual[i], expected[i]))
                    Asserter.Fail(string.Format(
                        "Property {0}.{1} does not match. Expected IDictionary with element {1} equals to {2} but was IDictionary with element {1} equals to {3}",
                        property.PropertyType.Name, property.Name, expected[i], actual[i]));
        }

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (actualList.Count != expectedList.Count)
                Asserter.Fail(
                    $"Property {property.PropertyType.Name}.{property.Name} does not match. Expected IList containing {expectedList.Count} elements but was IList containing {actualList.Count} elements");

            for (var i = 0; i < actualList.Count; i++)
                actualList[i].ShouldEqual(expectedList[i],
                    string.Format(
                        "Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}",
                        property.PropertyType.Name, property.Name, expectedList[i], actualList[i]));
        }
    }
}