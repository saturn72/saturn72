#region

using Saturn72.Core.Data;
using Saturn72.Core.Domain.Notifications;

#endregion

namespace Saturn72.Core.Services.Data.Repositories
{
    public interface INotificationRepository : IRepository<NotificationDomainModel, long>
    {
    }
}