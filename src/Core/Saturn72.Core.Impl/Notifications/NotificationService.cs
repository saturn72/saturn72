#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Notifications;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Notifications;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Notifications
{
    public class NotificationService : INotificationService
    {
        #region consts

        private const string AllNotificationCacheKey = "saturn72.notifications";
        private const string NotificationCacheKey = "saturn72.notifications-{0}";

        #endregion


        public NotificationService(INotificationRepository notificationRepository, IEventPublisher eventPublisher,
            ICacheManager cacheManager, AuditHelper auditHelper)
        {
            _notificationRepository = notificationRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
            _auditHelper = auditHelper;
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

        public async Task<NotificationDomainModel> CreateNotificationAsync(NotificationDomainModel notification)
        {
            Guard.NotNull(notification);
            _auditHelper.PrepareForCreateAudity(notification);
            await Task.Run(() => _notificationRepository.Create(notification));

            _eventPublisher.DomainModelCreated(notification);
            _cacheManager.RemoveByPattern(AllNotificationCacheKey);
            return notification;
        }

        public async Task<NotificationDomainModel> UpdateNotificationAsync(NotificationDomainModel notification)
        {
            Guard.NotNull(notification);
            _auditHelper.PrepareForUpdateAudity(notification);
            await Task.Run(() => _notificationRepository.Update(notification));

            _eventPublisher.DomainModelUpdated(notification);
            _cacheManager.RemoveByPattern(AllNotificationCacheKey);
            return notification;
        }

        public async Task DeleteNotificationAsync(long id)
        {
            Guard.GreaterThan(id, 0);
            NotificationDomainModel notification = null;

            await Task.Run(() =>
            {
                notification = _notificationRepository.GetById(id);
                _notificationRepository.Delete(id);
            });

            _eventPublisher.DomainModelDeleted(notification);
            _cacheManager.RemoveByPattern(AllNotificationCacheKey);
        }

        public async Task<NotificationDomainModel> GetNotificationByIdAsync(long id)
        {
            return
                await
                    Task.Run(
                        () =>
                            _cacheManager.Get(NotificationCacheKey.AsFormat(id),
                                () => _notificationRepository.GetById(id)));
        }

        #region Fields

        private readonly INotificationRepository _notificationRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly AuditHelper _auditHelper;

        #endregion
    }
}