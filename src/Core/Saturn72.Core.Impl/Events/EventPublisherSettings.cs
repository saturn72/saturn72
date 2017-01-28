
using Saturn72.Core.Configuration;

namespace Saturn72.Core.Services.Impl.Events
{
    public class EventPublisherSettings:SettingsBase
    {
        public int MaxNumberOfThreads { get; set; }
    }
}
