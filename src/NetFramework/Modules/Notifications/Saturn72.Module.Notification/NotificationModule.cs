#region Usings

using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.Notification
{
    public class NotificationModule : IModule
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