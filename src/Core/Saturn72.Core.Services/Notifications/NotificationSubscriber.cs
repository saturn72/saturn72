#region

using System.Collections.Generic;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.Notifications
{
    public class NotificationSubscriber
    {
        private ICollection<string> _notificationIds;
        public UserDomainModel UserDomainModel { get; set; }

        public virtual ICollection<string> NotificationIds
        {
            get { return _notificationIds ?? (_notificationIds = new List<string>()); }
        }
    }
}