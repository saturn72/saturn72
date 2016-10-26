#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Services.Events
{
    public interface ISubscriptionService
    {
        IEnumerable<IConsumer<TEvent>> GetSubscriptions<TEvent>() where TEvent : EventBase;
    }
}