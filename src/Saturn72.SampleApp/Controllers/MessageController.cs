using Microsoft.AspNetCore.Mvc;
using Saturn72.Core.Messaging;

namespace Saturn72.SampleApp.Controllers
{
    public enum MessageEnum
    {
        DirectBetweenEntities, // direct message from one person to another
        FromEntityToRole, // sends message from one person to another person by role
        FromEntityToDevices, // sends message fom entity to devices 
        FromDeviceToDevices, // sends message from device to devices 
    }

    [ApiController]
    [Route("message")]
    public class MessageController : ControllerBase
    {
        private readonly IMessager _messager;

        public MessageController(IMessager messager)
        {
            _messager = messager;
        }

        [HttpPost]
        public async Task<IActionResult> PalletStatus(PalletStatusModel model)
        {
            var message = new Message
            {
                MessageType = "maintenance",
                Key = "pallet-status",
                Version = "1",
                Payload = model
            };

            await _messager.SendAsync(message);
            return Accepted();
        }

        public record PalletStatusModel
        {
            public string? Area { get; init; } = "area-1";
            public int PalletCount { get; init; }
        }
    }
}