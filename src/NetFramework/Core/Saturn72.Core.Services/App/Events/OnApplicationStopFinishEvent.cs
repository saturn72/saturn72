using Saturn72.Core.Services.Events;

namespace Saturn72.Core.Services.App.Events
{
    public class OnApplicationStopFinishEvent : EventBase
    {
        public OnApplicationStopFinishEvent(IApp app)
        {
            App = app;
        }

        public IApp App { get; }
    }
}