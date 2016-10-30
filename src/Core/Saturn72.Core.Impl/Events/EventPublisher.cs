#region

using System;
using Saturn72.Core.Logging;
using Saturn72.Extensions;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Impl.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger _logger;

        public EventPublisher(ISubscriptionService subscriptionService, ILogger logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
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
            ////Ignore not installed plugins and mosules
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
                    _logger.Error(exc.Message, exc);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }
    }
}