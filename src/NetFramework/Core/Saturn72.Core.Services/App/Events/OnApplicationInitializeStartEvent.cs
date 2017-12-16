﻿using Saturn72.Core.Services.Events;

namespace Saturn72.Core.Services.App.Events
{
    public class OnApplicationInitializeStartEvent : EventBase
    {
        public OnApplicationInitializeStartEvent(IApp app)
        {
            App = app;
        }

        public IApp App { get; private set; }
    }
}