using System.Threading;
using System.Threading.Tasks;
using Saturn72.Core.Domain;

namespace Saturn72.Core.Services.Events
{
    public static class EventPublisherExtensions
    {
        public static void PublishToAll<TEvent>(this IEventPublisher eventPublisher, TEvent eventMessage)
        where TEvent : EventBase
        {
            eventPublisher.PublishAsync(eventMessage);
            Task.Run(() => eventPublisher.Publish(eventMessage));
        }
        public static void PublishAsync<TEvent>(this IEventPublisher eventPublisher, TEvent eventMessage)
        where TEvent : EventBase
        {
            eventPublisher.PublishAsync(eventMessage, CancellationToken.None);
        }

        public static void PublishToAllDomainModelCreatedEvent<TDomainModel>(this IEventPublisher eventPublisher, TDomainModel model)
            where TDomainModel : DomainModelBase
        {
            PublishToAll(eventPublisher, new DomainModelCreatedEvent<TDomainModel>
            {
                Model = model
            });
        }

        public static void PublishToAllDomainModelReadEvent<TDomainModel>(this IEventPublisher eventPublisher, TDomainModel model)
            where TDomainModel : DomainModelBase
        {
            PublishToAll(eventPublisher, new DomainModelReadEvent<TDomainModel>
            {
                Model = model
            });
        }
    }
}