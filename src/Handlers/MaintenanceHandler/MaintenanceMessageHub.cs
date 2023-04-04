
using MaintenanceHandler.Messages.Pallet;
using System.Text.Json;

namespace MaintenanceHandler
{
    public class MaintenanceMessageHub : MessageHandlerBase
    {
        private static readonly MessageEndpointDefinition PalletStatus = new() { Name = "pallet-status", Version = "1", };

        private readonly IReadOnlyDictionary<string, Func<ConsumeContext<Message>, Task>> _handlers;

        public MaintenanceMessageHub()
        {
            _handlers = new Dictionary<string, Func<ConsumeContext<Message>, Task>>()
            {
                { PalletStatus.GetIdentifier(),  HandlePalletStatusRequest },
            };
        }

        public override IReadOnlyCollection<MessageEndpointDefinition> EndpointDefinitions => new[]
        {
            PalletStatus,
        };

        public override async Task Consume(ConsumeContext<Message> context)
        {
            var identifier = context.Message.GetIdentifier();
            if (_handlers.TryGetValue(identifier, out var handler))
                await handler(context);
        }
        private async Task HandlePalletStatusRequest(ConsumeContext<Message> context)
        {
            //start saga
            //ask ai to estimate pallets number in area
            //get AI response
            //if above minimum amount - do nothing
            // else - send command to add pallets

            var je = (JsonElement)context.Message.Payload;

            if (!je.TryGetProperty("area", out var area))
                throw new NotImplementedException("handle error here");

            await context.Publish(new OnEstimatePalletRequested
            {
                Area = area.GetString()
            });
        }
    }
}