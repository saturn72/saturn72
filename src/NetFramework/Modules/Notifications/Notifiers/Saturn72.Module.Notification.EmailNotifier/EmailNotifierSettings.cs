
using System.Net;
using System.Net.Mail;
using Saturn72.Core.Configuration;

namespace Saturn72.Module.Notification.EmailNotifier
{
    public class EmailNotifierSettings:SettingsBase
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public ICredentialsByHost Credentials { get; set; }
        public string RazorViewsPath { get; set; }
    }
}
