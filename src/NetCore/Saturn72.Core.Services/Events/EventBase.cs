using System;

namespace Saturn72.Core.Services.Events
{
    public abstract class EventBase
    {
        public DateTime FiredOnUtc { get; internal set; }
        public DateTime CreatedOnUtc { get; }

        protected EventBase()
        {
            CreatedOnUtc = DateTime.UtcNow;
        }
    }
}