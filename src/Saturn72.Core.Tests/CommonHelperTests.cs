using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace Saturn72.Core.Tests
{
    public class CommonHelperTests

    {
        [Fact]
        public void CommonHelper_GetAssemblyLocalPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var expected = new Uri(asm.CodeBase).LocalPath;
            CommonHelper.GetAssemblyLocalPath(asm).ShouldBe(expected);
        }

        [Fact]
        public void CommonHelper_RunTimedoutExpression_TimeoutExceeds()
        {
            CommonHelper.RunTimedoutExpression(() => false, 100, 20).ShouldBeFalse();
        }

        [Fact]
        public void CommonHelper_RunTimedoutExpression_TrueExpression()
        {
            var i = 0;
            CommonHelper.RunTimedoutExpression(() => i++ == 5, 100, 10).ShouldBeTrue();
        }

        [Fact]
        public void CommonHelper_Copy_Throws()
        {
            //different type
            Should.Throw<InvalidOperationException>(
                () => CommonHelper.Copy(new DummyClass(), new DummyClassChild()));
        }

        [Fact]
        public void CommonHelper_Copy_CreateNew()
        {
            var source = new DummyClass();
            var dest = CommonHelper.Copy(source);

            dest.InternalString.ShouldBe(source.InternalString);
            dest.StringWithSetter.ShouldBe(source.StringWithSetter);
            dest.StringWithoutSetter.ShouldBe(source.StringWithoutSetter);
        }

        [Fact]
        public void CommonHelper_Copy_ToInstance()
        {
            var source = new DummyClass
            {
                StringWithSetter = "string with setter",
                InternalString = "internal string"
            };

            var dest = new DummyClass();

            CommonHelper.Copy(source, dest);

            dest.InternalString.ShouldBe(source.InternalString);
            dest.StringWithSetter.ShouldBe(source.StringWithSetter);
            dest.StringWithoutSetter.ShouldBe(source.StringWithoutSetter);
        }

        [Fact]
        public void CommonHelper_Copy_ProtectedProperties()
        {
            var source = new DummyClassChild
            {
                StringWithSetter = "string with setter",
                InternalString = "internal string"
            };
            source.SetProtectedString("value");

            var dest = new DummyClassChild();

            CommonHelper.Copy(source, dest);

            dest.InternalString.ShouldBe(source.InternalString);
            dest.StringWithSetter.ShouldBe(source.StringWithSetter);
            dest.StringWithoutSetter.ShouldBe(source.StringWithoutSetter);
        }


        internal class DummyClass
        {
            private const string _stringWithoutSetter = "TTT";

            public string StringWithSetter { get; set; }

            public string StringWithoutSetter
            {
                get { return _stringWithoutSetter; }
            }

            internal string InternalString { get; set; }
            protected string ProtectedString { get; set; }
        }

        internal class DummyClassChild : DummyClass
        {
            public string GetProtectedString
            {
                get { return ProtectedString; }
            }

            public void SetProtectedString(string value)
            {
                ProtectedString = value;
            }
        }
    }
}