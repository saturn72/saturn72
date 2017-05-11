using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Shouldly;

namespace Saturn72.TestUtil
{
    public static class UnitTestUtil
    {
        public static void PropertyValuesAreEquals<TType>(this TType actual, TType expected)

        {
            PropertyValuesAreEquals(actual, expected, new string[] { });
        }

        public static void PropertyValuesAreEquals<TType>(this TType actual, TType expected, string[] execludeProperties)
        {
            var properties = expected.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (execludeProperties.Contains(property.Name, StringComparer.InvariantCultureIgnoreCase))
                    continue;

                object expectedValue = property.GetValue(expected, null);
                object actualValue = property.GetValue(actual, null);

                if (actualValue is IDictionary)
                {
                    AssertDictionariesAreEquals(property, (IDictionary)actualValue, (IDictionary)expectedValue);
                    continue;
                }
                if (actualValue is IList)
                {
                    AssertListsAreEquals(property, (IList)actualValue, (IList)expectedValue);
                    continue;
                }

                if (!Equals(expectedValue, actualValue))
                {
                    Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}",
                        property.DeclaringType.Name, property.Name, expectedValue, actualValue);
                }
            }
        }

        private static void AssertDictionariesAreEquals(PropertyInfo property, IDictionary actual, IDictionary expected)
        {
            if (actual.Count != expected.Count)
                Assert.Fail(
                    "Property {0}.{1} does not match. Expected IDictionary containing {2} elements but was IDictionary containing {3} elements",
                    property.PropertyType.Name, property.Name, expected.Count, actual.Count);

            for (int i = 0; i < actual.Count; i++)
                if (!Equals(actual[i], expected[i]))
                    Assert.Fail(
                        "Property {0}.{1} does not match. Expected IDictionary with element {1} equals to {2} but was IDictionary with element {1} equals to {3}",
                        property.PropertyType.Name, property.Name, expected[i], actual[i]);
        }

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (actualList.Count != expectedList.Count)
                Assert.Fail(
                    "Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements",
                    property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);

            for (int i = 0; i < actualList.Count; i++)
                actualList[i].ShouldBe(expectedList[i],
                    string.Format(
                        "Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}",
                        property.PropertyType.Name, property.Name, expectedList[i], actualList[i]));
        }
    }
}