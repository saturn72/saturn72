namespace Saturn72.Core.Messaging
{
    public abstract class MessageHandlerBase : MessageHandlerBase<Message>
    {

    }
    public abstract class MessageHandlerBase<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        public abstract Task Consume(ConsumeContext<TMessage> context);
        public abstract IReadOnlyCollection<MessageEndpointDefinition> EndpointDefinitions { get; }
    }
}
