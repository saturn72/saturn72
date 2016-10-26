namespace Saturn72.Core.Services.Events
{
    public interface IEventPublisher
    {
        void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase;
    }
}