#region

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Threading.Tasks;
using System.Web.Mvc;
using Postal;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Domain.Configuration;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Notifications;
using Saturn72.Extensions;
using Saturn72.Module.Notification.EmailNotifier.Models;
using CommonHelper = Saturn72.Core.CommonHelper;

#endregion

namespace Saturn72.Module.Notification.EmailNotifier
{
    public class EmailNotifier : INotifier
    {
        private const string PickupdirectorylocationKey = "PickupDirectoryLocation";
        private readonly INotificationService _notificationService;
        private EmailNotifierSettings _settings;

        private ViewEngineCollection _viewEngines;

        public EmailNotifier(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Notify(NotificationMessage message)
        {
            var notificationKey = message.NotificationKey;
            var email = ToEmailMessageModel(message, notificationKey);

            var emailService = new EmailService(_viewEngines, () => GetSmtpClient(email));

            emailService.Send(email);
        }

        public Task NotifyAsync(NotificationMessage message)
        {
            return new Task(() => Notify(message));
        }


        public void Configure(ITypeFinder typeFinder)
        {
            ConfigureSmtpSettings();

            _viewEngines = new ViewEngineCollection
            {
                new FileSystemRazorViewEngine(_settings.RazorViewsPath)
            };
        }

        private EmailMessageModel ToEmailMessageModel(NotificationMessage message, string notificationKey)
        {
            return new EmailMessageModel(notificationKey)
            {
                From = _notificationService.GetSystemNotificationSender(notificationKey),
                Subject = message.Title,
                Message = message.Message ?? "",
                Content = message.Content,
                To = _notificationService
                    .GetNotificationSubscribers(notificationKey)
                    .Select(x => x.UserDomainModel.Email)
                    .Aggregate((current, next) => current + ", " + next)
            };
        }

        private void ConfigureSmtpSettings()
        {
            var config = ConfigManager.Current.ConfigMaps["EmailNotifierConfig"];
            _settings = new EmailNotifierSettings
            {
                Host = config.GetValueAsString("Host"),
                Port = CommonHelper.ToInt(config.GetValueAsString("Port")),
                EnableSsl = config.GetValueAsString("EnableSsl").ToBoolean()
            };

            //delivery method
            SmtpDeliveryMethod policy;
            Enum.TryParse(config.GetValueAsString("SmtpDeliveryMethod"), true, out policy);
            _settings.DeliveryMethod = policy;

            if (_settings.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                _settings.SettingEntries.Add(PickupdirectorylocationKey, new SettingEntryDomainModel
                {
                    Name = PickupdirectorylocationKey,
                    Value = config.GetValueAsString(PickupdirectorylocationKey)
                });

            //credentials
            var username = config.GetValueAsString("UsernameOrEmail");
            var password = config.GetValueAsString("Password");
            _settings.Credentials = SetCredentials(username, password);

            //Razor views path
            var viewsPath = config.GetValueAsString("EmailTemplatesPath");
            _settings.RazorViewsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, viewsPath);

            //Incase 
        }

        protected virtual SmtpClient GetSmtpClient(EmailMessageModel email)
        {
            var smtpClient = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = _settings.DeliveryMethod,
                Credentials = _settings.Credentials
            };
            //Support for file system delivery
            if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
            {
                var path =
                    FileSystemUtil.RelativePathToAbsolutePath(_settings.SettingEntries[PickupdirectorylocationKey].Value);
                smtpClient.PickupDirectoryLocation = path;
                FileSystemUtil.CreateDirectoryIfNotExists(path);
            }
            return smtpClient;
        }

        protected virtual ICredentialsByHost SetCredentials(string userName, string password)
        {
            SecureString securePassword;
            unsafe
            {
                fixed (char* passwordChars = password)
                {
                    securePassword = new SecureString(passwordChars, password.Length);
                    securePassword.MakeReadOnly();
                }
            }

            return new NetworkCredential(userName, securePassword);
        }
    }
}