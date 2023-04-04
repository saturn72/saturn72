namespace Saturn72.Core.Messaging
{
    public static class MessageExtensions
    {
        public static async Task SendAsync(this ISendEndpointProvider provider, Message message)
        {
            var uri = new Uri("queue:" + message.GetIdentifier());
            var endpoint = await provider.GetSendEndpoint(uri);
            await endpoint.Send(message);
        }

        public static string GetIdentifier(this MessageEndpointDefinition definition) =>
            GetIdentifier(definition.Name, definition.Version);

        public static string GetIdentifier(this Message message) =>
            GetIdentifier(message.Key, message.Version);

        private static string GetIdentifier(string key, string version) =>
            $"{nameof(key)}__{key}__{nameof(version)}__{version}";
    }
}