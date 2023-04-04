

namespace Saturn72.Core.Messaging
{
    public record Message
    {
        public string? Key { get; set; }
        public string? Version { get; set; }
        public string? MessageType { get; init; }
        public string? ContentType { get; init; } = "application/json";
        public object? Payload { get; init; }
    }
}