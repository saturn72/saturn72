#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class StringExtensionsTests
    {
        [Fact]
        public void HasValue_null()
        {
            string source = null;
            source.HasValue().ShouldBeFalse();
        }

        [Fact]
        public void HasValue_white_spaces()
        {
            "    ".HasValue().ShouldBeFalse();
        }

        [Fact]
        public void HasValue_with_value()
        {
            "test_string".HasValue().ShouldBeTrue();
        }

        [Fact]
        public void RemoveSubStringInstances_removes_all()
        {
            "AACC".ShouldEqual("AAbbCC".RemoveAllInstances("bb"));
            "AbCC".ShouldEqual("AAbbCC".RemoveAllInstances("Ab"));
            "bbCC".ShouldEqual("AAbbCC".RemoveAllInstances("AA"));
        }

        [Fact]
        public void RemoveSubStringInstances_removesnothing()
        {
            "AbbACC".ShouldEqual("AbbACC".RemoveAllInstances("AA"));
        }

        [Fact]
        public void RemoveAll_RemovesAllInstances1()
        {
            "aaaBBBbbbCc".ShouldEqual("AAAaaaBBBbbbCc".RemoveAllInstances("A"));
        }

        [Fact]
        public void RemoveAll_RemovesAllInstances2()
        {
            "aaaBc".ShouldEqual("AAAaaaBBBbbbCc".RemoveAllInstances("A", "BB", "bbb", "C"));
        }

        [Fact]
        public void RemoveAllWhiteSpaces()
        {
            "A".ShouldEqual("    A               ".RemoveAllWhiteSpaces());
        }

        [Fact]
        public void AsFormat_empty()
        {
            "    _logic".ShouldEqual("    {0}".AsFormat("_logic"));
        }

        [Fact]
        public void AsFormat_MissingArgThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => "test{0} {1}".AsFormat(1));
        }

        [Fact]
        public void AsFormat_string_and_object()
        {
            var o = new object();
            "test_logic".ShouldEqual("test{0}".AsFormat("_logic", o));
        }

        [Fact]
        public void AsFormat_strings()
        {
            "test_logic".ShouldEqual("test{0}".AsFormat("_logic"));
        }

        [Fact]
        public void AsFormat_ThrowesException()
        {
            Assert.Throws<FormatException>(() => "{ Test }".AsFormat("123"));
        }

        [Fact]
        public void AsFormat_Dictionary()
        {
            var formatDictionary = new Dictionary<string, object>
            {
                {"t1", "TTT"},
                {"t2", 2},
                {"t3", new object()}
            };

            "TTT 2 System.Object".ShouldEqual("{t1} {t2} {t3}".AsFormat(formatDictionary));
        }

        [Fact]
        public void Replace_CheckOutOfBoundIndexes()
        {
            "source".Replace(-1, 100, "ddd").ShouldEqual("source");
            "source".Replace(100, 1, "ddd").ShouldEqual("source");
            "source".Replace(2, 100, "ddd").ShouldEqual("soddd");
        }

        [Fact]
        public void Replace_Replaces()
        {
            "source".Replace(2, 4, "ddd").ShouldEqual("sodddce");
            "source".Replace(4, 4, "ddd").ShouldEqual("sourdddce");
        }

        [Fact]
        public void ToBoolean_ReturnsTrue()
        {
            "true".ToBoolean().ShouldBeTrue();

            "True".ToBoolean().ShouldBeTrue();
            "tRue".ToBoolean().ShouldBeTrue();
            "trUe".ToBoolean().ShouldBeTrue();
            "truE".ToBoolean().ShouldBeTrue();

            "TRue".ToBoolean().ShouldBeTrue();
            "TrUe".ToBoolean().ShouldBeTrue();
            "TruE".ToBoolean().ShouldBeTrue();
            "tRUe".ToBoolean().ShouldBeTrue();
            "tRuE".ToBoolean().ShouldBeTrue();
            "trUE".ToBoolean().ShouldBeTrue();

            "TRUe".ToBoolean().ShouldBeTrue();
            "TRuE".ToBoolean().ShouldBeTrue();
            "tRUE".ToBoolean().ShouldBeTrue();

            "TRUE".ToBoolean().ShouldBeTrue();
        }

        [Fact]
        public void ToBoolean_ReturnsFalse()
        {
            "false".ToBoolean().ShouldBeFalse();
            ((string) null).ToBoolean().ShouldBeFalse();
            "".ToBoolean().ShouldBeFalse();
            "notTrue".ToBoolean().ShouldBeFalse();
        }

        [Fact]
        public void EqualsToIgnoreCases_AllTests()
        {
            const string source = "ThisIs String";
            var equals = new[]
            {
                "thisis string",
                "thisis String",
                "THISIS STRING"
            };

            var notEquals = new[]
            {
                "this isstrin",
                "Thisisstring",
                "SHT SIIIS "
            };

            foreach (var val in equals)
                source.EqualsTo(val).ShouldBeTrue();

            foreach (var val in notEquals)
                source.EqualsTo(val).ShouldBeFalse();
        }

        [Fact]
        public void RemoveNewLineEscape_ClearsNewLinesEscapes()
        {
            var source = "This\n is\nsource\n";
            Assert.AreEqual("This is source ", source.RemoveNewLineEscape());

        }

        [Fact]
        public void RemoveNewLineEscape_ReturnsSourceOnNoNewLineEscapes()
        {
            var source = "This is source";

            Assert.AreEqual("This is source", source.RemoveNewLineEscape());
        }

        [Fact]
        public void IsUrl_returnsTrueOnHttpUrl()
        {
            Assert.True("https://www.test.com".IsUrl());
        }

        [Fact]
        public void IsUrl_returnsTrueOnHttpsUrl()
        {
            Assert.True("http://www.test.com".IsUrl());
        }

        [Fact]
        public void IsUrl_returnsTrueOnFileSystemUrlUrl()
        {
            Assert.True("file:///c:/WINDOWS/clock.avi".IsUrl());
        }

        [Fact]
        public void IsUrl_ReturnsFalseOnEmptyString()
        {
            Assert.False("".IsUrl());
            Assert.False(((string)null).IsUrl());
        }

        [Fact]
        public void IsUrl_ReturnsFalseOnIllegalUrl()
        {
            Assert.False("This is not url".IsUrl());
        }
    }
}