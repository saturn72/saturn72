
namespace Saturn72.Core.Messaging
{
    public class MassTransitMessager : IMessager
    {
        private readonly IBus _bus;

        public MassTransitMessager(IBus bus)
        {
            _bus = bus;
        }
        public async Task SendAsync(Message message) =>
            await _bus.SendAsync(message);
    }
}
