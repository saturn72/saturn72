#region

using System;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionService _subscriptionService;

        public EventPublisher(ISubscriptionService subscriptionService, ILogger logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        public void Publish<TEvent>(TEvent eventMessage) where TEvent : EventBase
        {
            //Async subscribers
            throw new NotImplementedException("fire and forget to async");
            var asyncSubscribers = _subscriptionService.GetAsyncSubscriptions<TEvent>();


            //TODO: ===> this runs synced, wrap with async executor that gets max number of thread to use
            asyncSubscribers.ForEachItem(x => PublishToConsumer(async ()=> await x.HandleEvent(eventMessage)));
            
            //synced subscribers
            var subscriptions = _subscriptionService.GetSubscriptions<TEvent>();
            subscriptions.ForEachItem(x => PublishToConsumer(()=> x.HandleEvent(eventMessage)));
        }

        protected virtual void PublishToConsumer(Action action)
        {
            //TODO: add addons (plugin support)
            ////Ignore not installed plugins
            //var plugin = FindPlugin(consumer.GetType());
            //if (plugin != null && !plugin.State)
            //    return;

            try
            {
               action();
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