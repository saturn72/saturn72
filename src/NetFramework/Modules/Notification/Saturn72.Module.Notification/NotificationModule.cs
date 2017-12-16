﻿#region

using System.Collections.Generic;
using Saturn72.Extensions;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Services;
using Saturn72.Core.Services.Notifications;

#endregion

namespace Saturn72.Module.Notification
{
    public class NotificationModule : ResolverBase, IModule
    {
        public void Load(IDictionary<string, IConfigMap> configurations)
        {
            ResolveAll<INotifier>().ForEachItem(n => n.Configure(TypeFinder));
        }

        public void Start(IDictionary<string, IConfigMap> configuration)
        {
        }

        public void Stop(IDictionary<string, IConfigMap> configurations)
        {
        }
    }
}