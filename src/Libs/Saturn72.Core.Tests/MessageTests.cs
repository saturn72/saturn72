
namespace Saturn72.Core.Messaging.Tests
{
    public class MessageTests
    {
        [Fact]
        public void DefaultValues()
        {
            var msg = new Message();
            msg.ContentType.ShouldBe("application/json");
        }
    }
}