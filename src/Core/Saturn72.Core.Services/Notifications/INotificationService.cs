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
        IEnumerable<NotificationDomainModel> GetAllNotifications();
        Task<NotificationDomainModel> CreateNotificationAsync(NotificationDomainModel notification);
        Task<NotificationDomainModel> UpdateNotificationAsync(NotificationDomainModel notification);
        void DeleteNotification(long id);
        NotificationDomainModel GetNotificationById(long id);
    }
}