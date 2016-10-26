#region

using System.Collections.Generic;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Notifications
{
    public class NotificationMessage
    {
        public NotificationMessage(string notificationKey, string title, string message,
            IDictionary<string, object> content)
        {
            NotificationKey = notificationKey;
            Title = title;
            Message = message;
            Content = content;

            if (content.NotNull())
                Status = content.GetValueOrDefault("Status");
        }

        public object Status { get; set; }
        public string NotificationKey { get; private set; }
        public string Message { get; private set; }
        public string Title { get; private set; }
        public IDictionary<string, object> Content { get; private set; }
    }
}