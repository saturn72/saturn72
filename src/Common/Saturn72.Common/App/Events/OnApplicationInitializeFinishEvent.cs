using Saturn72.Core.Services.Events;

namespace Saturn72.Common.App.Events
{
    public class OnApplicationInitializeFinishEvent : EventBase
    {
        public OnApplicationInitializeFinishEvent(IApp app)
        {
            App = app;
        }
        public IApp App { get; private set; }

    }
}