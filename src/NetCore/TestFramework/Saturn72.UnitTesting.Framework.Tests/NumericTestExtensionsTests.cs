using Xunit;

namespace Saturn72.UnitTesting.Framework.Tests
{
    public class NumericTestExtensions
    {
        [Fact]
        public void ShouldBeGreaterThan()
        {
            //int
            3.ShouldBeGreaterThan(2);

            //long
            long.MaxValue.ShouldBeGreaterThan(long.MaxValue - 1);

            //decimal
            decimal.MaxValue.ShouldBeGreaterThan(decimal.MaxValue - 1);
        }

        [Fact]
        public void ShouldBeGreaterOrEqualTo_Int()
        {
            //int
            3.ShouldBeGreaterOrEqualTo(3);
            3.ShouldBeGreaterOrEqualTo(2);

            //long
            long.MaxValue.ShouldBeGreaterOrEqualTo(long.MaxValue);
            long.MaxValue.ShouldBeGreaterOrEqualTo(long.MinValue);

            //decimal
            decimal.MaxValue.ShouldBeGreaterOrEqualTo(decimal.MinValue);
            decimal.MaxValue.ShouldBeGreaterOrEqualTo(decimal.MaxValue);
        }

        [Fact]
        public void ShouldBeSmallerThan()
        {
            //int
            2.ShouldBeSmallerThan(3);
            //long
            long.MinValue.ShouldBeSmallerThan(long.MaxValue);

            //decimal
            decimal.MinValue.ShouldBeSmallerThan(decimal.MaxValue);
        }

        [Fact]
        public void ShouldBeSmallerOrEqualTo()
        {
            //int
            3.ShouldBeSmallerOrEqualTo(3);
            1.ShouldBeSmallerOrEqualTo(2);

            //long
            long.MinValue.ShouldBeSmallerOrEqualTo(long.MaxValue);
            long.MinValue.ShouldBeSmallerOrEqualTo(long.MinValue);

            //decimal
            decimal.MinValue.ShouldBeSmallerOrEqualTo(decimal.MinValue);
            decimal.MinValue.ShouldBeSmallerOrEqualTo(decimal.MaxValue);
        }
    }
}