

namespace Saturn72.Core.Messaging
{
    public class DaprMessager : IMessager
    {
        private readonly DaprClient _daprClient;

        public DaprMessager(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }
        public async Task SendAsync(Message message) =>
            await _daprClient.InvokeMethodAsync("catalog", "handlemessage");
    }
}
