namespace Saturn72.Core.Services.Events
{
    public interface IConsumer<in TEvent> where TEvent : EventBase
    {
        void HandleEvent(TEvent eventMessage);
    }
}