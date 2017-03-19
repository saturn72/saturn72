#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Notifications;

#endregion

namespace Saturn72.Core.Services.Notifications
{
    public interface INotificationService
    {
        IEnumerable<NotificationSubscriber> GetNotificationSubscribers(string notificationKey);
        string GetSystemNotificationSender(string notificationKey);
        IEnumerable<NotificationModel> GetAllNotifications();
        Task<NotificationModel> CreateNotificationAsync(NotificationModel notification);
        Task<NotificationModel> UpdateNotificationAsync(NotificationModel notification);
        Task DeleteNotificationAsync(long id);
        Task<NotificationModel> GetNotificationByIdAsync(long id);
    }
}