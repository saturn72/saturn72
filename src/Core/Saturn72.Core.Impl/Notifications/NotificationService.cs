#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Data.Repositories;
using Saturn72.Core.Domain.Notifications;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Notifications;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Notifications
{
    public class NotificationService : DomainModelCrudServiceBase<NotificationDomainModel, long, long>, INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository, IEventPublisher eventPublisher, ICacheManager cacheManager, ITypeFinder typeFinder, IWorkContext<long> workContext)
            : base(notificationRepository, eventPublisher, cacheManager, typeFinder, workContext)
        {
            _notificationRepository = notificationRepository;
        }

        public IEnumerable<NotificationSubscriber> GetNotificationSubscribers(string notificationKey)
        {
            throw new NotImplementedException();

            return new[]
            {
                new NotificationSubscriber
                {
                    UserDomainModel = new UserDomainModel {Email = "roi@re-sec.com"}
                }
                //new NotificationSubscriber
                //{
                //    UserDomainModel = new UserDomainModel {Email = "alex@re-sec.com"},
                //}
            };
        }

        public string GetSystemNotificationSender(string notificationKey)
        {
            throw new NotImplementedException();

            return "saturn72test@gmail.com";
        }

        public IEnumerable<NotificationDomainModel> GetAllNotifications()
        {
            return _notificationRepository.GetAll();
        }

        public Task<NotificationDomainModel> CreateNotificationAsync(NotificationDomainModel notification)
        {
            Guard.NotNull(notification);
            return CreateAsync(notification);
        }

        public Task<NotificationDomainModel> UpdateNotificationAsync(NotificationDomainModel notification)
        {
            Guard.NotNull(notification);
            return UpdateAsync(notification);
        }

        public void DeleteNotification(long id)
        {
            Delete(id);
        }

        public NotificationDomainModel GetNotificationById(long id)
        {
            return GetById(id);
        }
    }
}