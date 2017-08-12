#region

using System;
using fastJSON;
using Saturn72.Core.Domain;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Events
{
    public static class EventPublisherExtensions
    {
        public static void DomainModelUpdated<TDomainModel>(this IEventPublisher eventPublisher,
            TDomainModel domainModel)
            where TDomainModel : DomainModelBase
        {
            var eventMessage = new UpdatedEvent<TDomainModel>(domainModel);
            eventPublisher.Publish(eventMessage);
        }

        public static void DomainModelCreated<TDomainModel>(this IEventPublisher eventPublisher,
            TDomainModel domainModel)
            where TDomainModel : DomainModelBase
        {
            var eventMessage = new CreatedEvent<TDomainModel>(domainModel);
            eventPublisher.Publish(eventMessage);
        }
        public static void DomainModelCreatedError<TDomainModel>(this IEventPublisher eventPublisher,
           TDomainModel domainModel, string errorMessage, Exception exception)
           where TDomainModel : DomainModelBase
        {
            var eventMessage = new CreatedErrorEvent<TDomainModel>(domainModel, errorMessage, exception);
            eventPublisher.Publish(eventMessage);
        }
        
        public static void DomainModelDeleted<TDomainModel>(this IEventPublisher eventPublisher,
            TDomainModel domainModel)
            where TDomainModel : DomainModelBase
        {
            var eventMessage = new DeletedEvent<TDomainModel>(domainModel);
            eventPublisher.Publish(eventMessage);
        }
    }
}