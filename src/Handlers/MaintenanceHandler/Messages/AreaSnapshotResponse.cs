namespace MaintenanceHandler.Messages
{
    public record AreaSnapshotResponse :
        CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; init; }
        public string? Area { get; init; }
        public byte[]? Bytes { get; init; }
    }
}
