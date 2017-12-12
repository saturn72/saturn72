#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services;
using Saturn72.Core.Services.Notifications;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Modules.NotificationManager
{
    public class NotificationManagerModule : IModule
    {
        public void Load()
        {
            var typeFinder = AppEngine.Current.Resolve<ITypeFinder>();
            AppEngine.Current.ResolveAll<INotifier>().ForEachItem(n => n.Configure(typeFinder));
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}