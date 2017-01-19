#region

using NUnit.Framework;

#endregion

namespace Saturn72.Extensions.UT
{
    public class ToolBoxTests
    {
        [Test]
        public void RunTimedoutExpression_TimeoutExceeds()
        {
            Assert.False(Toolbox.RunTimedoutExpression(() => false, 100, 20));
        }

        [Test]
        public void RunTimedoutExpression_TrueExpression()
        {
            var i = 0;
            Assert.True(Toolbox.RunTimedoutExpression(() => i++ == 5, 100, 10));
        }
    }
}