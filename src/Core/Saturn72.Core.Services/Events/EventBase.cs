#region

using System;

#endregion

namespace Saturn72.Core.Services.Events
{
    public abstract class EventBase
    {
        protected EventBase()
        {
            FiredOnUtc = DateTime.UtcNow;
        }

        public DateTime FiredOnUtc { get; set; }
    }
}