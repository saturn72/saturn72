#region

using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Notifications;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Module.Notification.EmailNotifier.Tests
{
    public class EmailNotifierTests
    {
        [Test]
        [Ignore("Requres abstraction on top of Post service, need to be fixed as part of email notification feature")]
        [Category("ignored")]
        public void Notify_SendsEmail()
        {
            var smtpTestConfig = new SmtpTestConfig();
            var emailPath =
                FileSystemUtil.RelativePathToAbsolutePath(smtpTestConfig.GetValueAsString("PickupDirectoryLocation"));
            FileSystemUtil.DeleteDirectoryContent(emailPath);

            ConfigManager.Current.ConfigMaps.Add("EmailNotifierConfig", smtpTestConfig);
            var t = "DDD";
            var notificationServiceMock = CreateMockForNotificationService();
            var notifier = new EmailNotifier(notificationServiceMock);

            notifier.Configure(null);
            var message = new NotificationMessage("Example", "This is notification title",
                "This is notification content", null);

            notifier.Notify(message);

            Thread.Sleep(1000);
            Directory.GetFiles(emailPath).Length.ShouldEqual(1);
        }

        private INotificationService CreateMockForNotificationService()
        {
            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.GetNotificationSubscribers(It.IsAny<string>()))
                .Returns(new[]
                {
                    new NotificationSubscriber {UserDomainModel = new UserDomainModel {Email = "subscriber1@email.com"}},
                    new NotificationSubscriber {UserDomainModel = new UserDomainModel {Email = "subscriber2@email.com"}}
                });

            notificationServiceMock.Setup(x => x.GetSystemNotificationSender(It.IsAny<string>()))
                .Returns("saturn72_test@gmail.com");

            return notificationServiceMock.Object;
        }
    }
}