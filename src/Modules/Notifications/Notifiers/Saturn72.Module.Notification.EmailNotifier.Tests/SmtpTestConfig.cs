#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;

#endregion

namespace Saturn72.Module.Notification.EmailNotifier.Tests
{
    public class SmtpTestConfig : IConfigMap
    {
        private readonly IDictionary<string, object> _smtpConfig = new Dictionary<string, object>
        {
            {"Host", "localhost"},
            {"Port", "25"},
            {"EnableSsl", "false"},
            //{"SmtpDeliveryMethod", "network"},
            {"SmtpDeliveryMethod", "SpecifiedPickupDirectory"},
            {"PickupDirectoryLocation", "Notifications\\Emails"},
            {"UsernameOrEmail", "N/A"},//"saturn72test@gmail.com"},
            {"Password", "N/A"}, //"!Q@W#E$R%"},
            {"EmailTemplatesPath", "EmailTemplates"}
        };

        public IDictionary<string, object> AllConfigRecords
        {
            get { return _smtpConfig; }
        }

        public object GetValue(string key)
        {
            return _smtpConfig[key];
        }
    }
}