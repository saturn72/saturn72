#region

using System.Collections.Generic;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Impl.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        public IEnumerable<IConsumer<TEvent>> GetSubscriptions<TEvent>() where TEvent : EventBase
        {
            return AppEngine.Current.ResolveAll<IConsumer<TEvent>>();
        }
    }
}