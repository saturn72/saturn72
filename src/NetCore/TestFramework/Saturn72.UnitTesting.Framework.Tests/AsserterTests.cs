using Xunit;

namespace Saturn72.UnitTesting.Framework.Tests
{
    public class AssertorTests
    {

       [Fact]
        public void Asserter_AreEqual()
        {
            Asserter.AreEqual(1, 1);
            Asserter.AreEqual(1, 1, "message");
        }


       [Fact]
        public void Asserter_Greater()
        {
            Asserter.Greater(2, 1);
            Asserter.Greater(2, 1, "message");
        }

       [Fact]
        public void Asserter_GreaterOrEqual()
        {
            Asserter.GreaterOrEqual(2, 1);
            Asserter.GreaterOrEqual(2, 1, "message");
            Asserter.GreaterOrEqual(2, 2);
            Asserter.GreaterOrEqual(2, 2, "message");
        }

       [Fact]
        public void Asserter_Smaller()
        {
            Asserter.Smaller(2, 3);
            Asserter.Smaller(2, 3, "message");
        }

       [Fact]
        public void Asserter_SmallerOrEqual()
        {
            Asserter.SmallerOrEqual(2, 3);
            Asserter.SmallerOrEqual(2, 3, "message");
            Asserter.SmallerOrEqual(2, 2);
            Asserter.SmallerOrEqual(2, 2, "message");
        }

       [Fact]
        public void True_NoMessage()
        {
            Asserter.True(true);
            Asserter.True(true, "message");
        }
    }
}