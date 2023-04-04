namespace MaintenanceHandler.Messages.Pallet
{
    public record OnEstimatePalletRequested :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string? Area { get; init; }
    }
}