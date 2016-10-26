#region

using System.Collections.Generic;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Impl.Events
{
    public class SubscriptionService : ResolverBase, ISubscriptionService
    {
        public IEnumerable<IConsumer<TEvent>> GetSubscriptions<TEvent>() where TEvent : EventBase
        {
            return ResolveAll<IConsumer<TEvent>>();
        }
    }
}