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
        public static void DomainModelUpdated<TDomainModel, TId>(this IEventPublisher eventPublisher,
           TDomainModel domainModel)
           where TDomainModel : DomainModelBase<TId>
        {
            var eventMessage = new UpdatedEvent<TDomainModel, TId>(domainModel);
            eventPublisher.Publish(eventMessage);
        }

        public static void DomainModelCreated<TDomainModel, TId>(this IEventPublisher eventPublisher,
            TDomainModel domainModel)
            where TDomainModel : DomainModelBase<TId>
        {
            var eventMessage = new CreatedEvent<TDomainModel, TId>(domainModel);
            eventPublisher.Publish(eventMessage);
        }

        public static void DomainModelDeleted<TDomainModel, TId>(this IEventPublisher eventPublisher,
            TDomainModel domainModel)
            where TDomainModel : DomainModelBase<TId>
        {
            var eventMessage = new DeletedEvent<TDomainModel, TId>(domainModel);
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
            var eventBaseType = typeof (EventBase);
            Guard.MustFollow(eventBaseType.IsAssignableFrom(type),
                "The sent type does not derive from " + eventBaseType.Name);

            var o = JsonConvert.DeserializeObject(value, type);
        }
    }
}