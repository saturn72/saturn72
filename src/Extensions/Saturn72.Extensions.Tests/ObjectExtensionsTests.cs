#region

using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.UT
{
    public class ObjectExtensionsTests
    {
        [Test]
        public void IsNull_ReturnsTrueOnNullObject()
        {
            ((object) null).IsNull().ShouldBeTrue();
        }

        [Test]
        public void IsNull_ReturnsFalseOnReferencedObject()
        {
            (new object()).IsNull().ShouldBeFalse();
        }

        [Test]
        public void NotNull_ReturnsFalseOnNullObject()
        {
            ((object) null).NotNull().ShouldBeFalse();
        }

        [Test]
        public void NotNull_ReturnsTrueOnReferencedObject()
        {
            (new object()).NotNull().ShouldBeTrue();
        }

        [Test]
        public void IsDefault_ReturnsFalseOnNonDefaultValueType()
        {
            1.IsDefault().ShouldBeFalse();
        }

        [Test]
        public void IsDefault_ReturnsFalseOnNonDefaultReferenceType()
        {
            (new object()).IsDefault().ShouldBeFalse();
        }

        [Test]
        public void IsDefault_ReturnsTrueOnDefaultValueType()
        {
            0.IsDefault().ShouldBeTrue();
        }

        [Test]
        public void IsDefault_ReturnsTrueOnDefaultReferenceType()
        {
            ((object)null).IsDefault().ShouldBeTrue();
        }

    }
}