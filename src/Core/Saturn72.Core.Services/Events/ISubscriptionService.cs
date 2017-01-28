#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Services.Events
{
    public interface ISubscriptionService
    {
        IEnumerable<IEventSubscriber<TEvent>> GetSubscriptions<TEvent>() where TEvent : EventBase;

        IEnumerable<IEventAsyncSubscriber<TEvent>> GetAsyncSubscriptions<TEvent>() where TEvent : EventBase;

    }
}