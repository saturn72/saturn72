namespace Saturn72.Core.Messaging
{
    public record MessageEndpointDefinition
    {
        public string? Name { get; init; }
        public string? Version { get; init; }
    }
}
