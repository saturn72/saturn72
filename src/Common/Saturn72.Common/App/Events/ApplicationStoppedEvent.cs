using Saturn72.Core.Services.Events;

namespace Saturn72.Common.App.Events
{
    public class ApplicationStoppedEvent : EventBase
    {
        public ApplicationStoppedEvent(IApp app)
        {
            App = app;
        }

        public IApp App { get; private set; }
    }
}