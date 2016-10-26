#region

using System.Threading.Tasks;
using Saturn72.Core.Extensibility;

#endregion

namespace Saturn72.Core.Services.Notifications
{
    public interface INotifier : IConfigurable
    {
        void Notify(NotificationMessage message);

        Task NotifyAsync(NotificationMessage message);
    }
}