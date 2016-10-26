#region

using System;
using Saturn72.Core.Logging;
using Saturn72.Extensions;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Impl.Events
{
    public class EventPublisher : Resolver, IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;

        public EventPublisher(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase
        {
            var subscriptions = _subscriptionService.GetSubscriptions<TEvent>();
            subscriptions.ForEachItem(x => PublishToConsumer(x, eventMessage));
        }

        protected virtual void PublishToConsumer<TEvent>(IConsumer<TEvent> consumer, TEvent eventMessage)
            where TEvent : EventBase
        {
            //TODO: add addons (plugin support)
            ////Ignore not installed plugins
            //var plugin = FindPlugin(consumer.GetType());
            //if (plugin != null && !plugin.State)
            //    return;

            try
            {
                consumer.HandleEvent(eventMessage);
            }
            catch (Exception exc)
            {
                //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
                try
                {
                    Logger.Error(exc.Message, exc);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }
    }
}