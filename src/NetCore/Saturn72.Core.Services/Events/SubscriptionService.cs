#region

using System.Collections.Generic;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        //TODO: filter plugins and modules that are not part of the runtime domain
        public IEnumerable<IEventSubscriber<TEvent>> GetSyncedSubscriptions<TEvent>() where TEvent : EventBase
        {
            return new List<IEventSubscriber<TEvent>>();
        }

        public IEnumerable<IEventAsyncSubscriber<TEvent>> GetAsyncSubscriptions<TEvent>() where TEvent : EventBase
        {
            return new List<IEventAsyncSubscriber<TEvent>>();
        }
    }
}