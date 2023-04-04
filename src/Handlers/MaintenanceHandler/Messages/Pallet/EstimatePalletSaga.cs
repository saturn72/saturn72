using MaintenanceHandler.Services;

namespace MaintenanceHandler.Messages.Pallet
{
    public class EstimatePalletSaga :
        ISaga,
        InitiatedBy<OnEstimatePalletRequested>
    {
        public Guid CorrelationId { get; set; }

        public Task Consume(ConsumeContext<OnEstimatePalletRequested> context)
        {
            var cameras = context.CreateInstance<ICameraService>();
            throw new NotImplementedException();
            //   await _cameraService.TakeSnapshot(context.Message.Area);
        }
    }
}