#region

using System.Collections.Generic;
using Postal;

#endregion

namespace Saturn72.Module.Notification.EmailNotifier.Models
{
    public class EmailMessageModel : Email
    {
        #region Fields

        private const string ToKey = "To";

        private const string FromKey = "From";

        private const string SubjectKey = "Subject";

        private const string MessageKey = "Message";

        #endregion

        #region Ctor

        public EmailMessageModel(string viewName) : base(viewName)
        {
        }

        #endregion


        public string To
        {
            get { return GetViewDataValueAsString(ToKey); }
            set { SetViewDataValue(ToKey, value); }
        }

        public string From
        {
            get { return GetViewDataValueAsString(FromKey); }
            set { SetViewDataValue(FromKey, value); }
        }

        public string Subject
        {
            get { return GetViewDataValueAsString(SubjectKey); }
            set { SetViewDataValue(SubjectKey, value); }
        }

        public string Message
        {
            get { return GetViewDataValueAsString(MessageKey); }
            set { SetViewDataValue(MessageKey, value); }
        }

        public IDictionary<string, object> Content { get; set; }
        public object Status { get; set; }

        #region Utilities

        private string GetViewDataValueAsString(string key)
        {
            return GetViewDataValue(key).ToString();
        }

        private object GetViewDataValue(string key)
        {
            return ViewData[key];
        }

        private void SetViewDataValue(string key, object value)
        {
            ViewData[key] = value;
        }

        #endregion
    }
}