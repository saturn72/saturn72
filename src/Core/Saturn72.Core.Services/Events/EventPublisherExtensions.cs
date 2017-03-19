#region

using System;
using Newtonsoft.Json;
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


        /// <summary>
        ///     Publishes event from json
        /// </summary>
        /// <param name="value"></param>
        /// <param name="typeFullName">
        ///     Type full name. Use the following format: {type_full_name}, {assembly_name}.
        ///     <example>System.string, mscorlib</example>
        /// </param>
        public static void Publish(string value, string typeFullName)
        {
            var type = Type.GetType(typeFullName);
            var eventBaseType = typeof(EventBase);
            Guard.MustFollow(eventBaseType.IsAssignableFrom(type),
                "The sent type does not derive from " + eventBaseType.Name);

            var o = JsonConvert.DeserializeObject(value, type);
        }
    }
}