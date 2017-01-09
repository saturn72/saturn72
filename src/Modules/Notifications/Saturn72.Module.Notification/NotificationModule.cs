#region

using System.Collections.Generic;
using Saturn72.Extensions;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services;
using Saturn72.Core.Services.Notifications;

#endregion

namespace Saturn72.Module.Notification
{
    public class NotificationModule : IModule
    {
        public void Load(IDictionary<string, IConfigMap> configurations)
        {
            var typeFinder = AppEngine.Current.Resolve<ITypeFinder>();
            AppEngine.Current.ResolveAll<INotifier>().ForEachItem(n => n.Configure(typeFinder));
        }

        public void Start(IDictionary<string, IConfigMap> configuration)
        {
        }

        public void Stop(IDictionary<string, IConfigMap> configurations)
        {
        }
    }
}