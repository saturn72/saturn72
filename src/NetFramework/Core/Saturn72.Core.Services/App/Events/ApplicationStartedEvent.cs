using Saturn72.Core.Services.Events;

namespace Saturn72.Core.Services.App.Events
{
    public class ApplicationStartedEvent : EventBase
    {
        public ApplicationStartedEvent(IApp app)
        {
            App = app;
        }

        public IApp App { get; private set; }
    }
}