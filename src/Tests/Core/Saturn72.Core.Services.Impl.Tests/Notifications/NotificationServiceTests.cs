#region

using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Notifications;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Notifications;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Notifications
{
    public class NotificationServiceTests
    {
        [Test]
        public void NotificationService_GetById_Throws()
        {
            var notificationService = new NotificationService(null, null, null);
            typeof(InvalidOperationException).ShouldBeThrownBy(
                () => notificationService.GetNotificationByIdAsync(0).RunSynchronously());
            typeof(InvalidOperationException).ShouldBeThrownBy(() =>
                notificationService.GetNotificationByIdAsync(-10).RunSynchronously());
        }


        [Test]
        public void NotificationService_GetById()
        {
            const int notificationId = 4;
            var ndm = new NotificationDomainModel
            {
                Id = notificationId,
                Name = "this is name",
                UpdatedOnUtc = DateTime.Now,
                CreatedOnUtc = DateTime.Now
            };

            var nRepo = new Mock<INotificationRepository>();

            nRepo.Setup(nr => nr.GetById(It.IsAny<long>()))
                .Returns(ndm);
            var srv = new NotificationService(nRepo.Object, null, null);
            var result = srv.GetNotificationByIdAsync(notificationId).Result;
            result.PropertyValuesAreEquals(ndm, new string[] {});
        }

        [Test]
        public void NotificationService_GetAllNotifications_ReturnsCollection()
        {
            var nRepo = new Mock<INotificationRepository>();
            var allNotifications = new[]
            {
                new NotificationDomainModel {Id = 1},
                new NotificationDomainModel {Id = 2},
                new NotificationDomainModel {Id = 3},
                new NotificationDomainModel {Id = 4}
            };
            nRepo.Setup(nr => nr.GetAll())
                .Returns(allNotifications);
            var srv = new NotificationService(nRepo.Object, null, null);
            var notifications = srv.GetAllNotifications();

            notifications.Count().ShouldEqual(4);
            for (var i = 0; i < notifications.Count(); i++)
                notifications.ElementAt(i).PropertyValuesAreEquals(allNotifications[i], new string[] {});
        }

        [Test]
        public void NotificationService_CreateNotification_ThrowsOnNull()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(
                () => new NotificationService(null, null, null).CreateNotificationAsync(null));
        }

        [Test]
        public void NotificationService_CreateNotification_Creates()
        {
            var ndm = new NotificationDomainModel
            {
                Name = "This is notificaiton"
            };
            const int expectedId = 99;
            var repo = new Mock<INotificationRepository>();
            repo.Setup(r => r.Create(It.IsAny<NotificationDomainModel>()))
                .Returns<NotificationDomainModel>(n =>
                {
                    n.Id = expectedId;
                    n.Name = ndm.Name;
                    return n;
                });

            var ePublisher = new Mock<IEventPublisher>();

            var srv = new NotificationService(repo.Object, ePublisher.Object, null);
            var task = srv.CreateNotificationAsync(ndm);
            task.Wait();
            var result = task.Result;

            result.Id.ShouldEqual(expectedId);
            result.PropertyValuesAreEquals(ndm, new[] {"Id"});
            (result.CreatedOnUtc != default(DateTime)).ShouldBeTrue();
            (result.UpdatedOnUtc >= result.CreatedOnUtc).ShouldBeTrue();

            ePublisher.Verify(e => e.Publish(It.IsAny<CreatedEvent<NotificationDomainModel>>()), Times.Exactly(1));
        }

        [Test]
        public void NotificationService_UpdateNotification_Throws()
        {
            //null object
            typeof(NullReferenceException).ShouldBeThrownBy(
                () => new NotificationService(null, null, null).UpdateNotificationAsync(null));

            var repo = new Mock<INotificationRepository>();
            repo.Setup(x => x.Update(It.IsAny<NotificationDomainModel>()))
                .Throws<ArgumentException>();

            var ePublisher = new Mock<IEventPublisher>();

            //missing Id/default
            var ndm = new NotificationDomainModel
            {
                Name = "This is new name"
            };

            typeof(AggregateException).ShouldBeThrownBy(
                () =>
                {
                    var t =
                        new NotificationService(repo.Object, ePublisher.Object, null).UpdateNotificationAsync(ndm)
                            .Result;
                });

            //Id not exists
            ndm.Id = long.MaxValue;
            typeof(AggregateException).ShouldBeThrownBy(
                () =>
                {
                    var t =
                        new NotificationService(repo.Object, ePublisher.Object, null).UpdateNotificationAsync(ndm)
                            .Result;
                });
        }

        [Test]
        public void NotificationService_UpdateNotification_Updates()
        {
            var ndm = new NotificationDomainModel
            {
                Id = 99,
                Name = "This is new notificaiton value"
            };
            var repo = new Mock<INotificationRepository>();
            var ePublisher = new Mock<IEventPublisher>();

            var srv = new NotificationService(repo.Object, ePublisher.Object, null);
            var task = srv.UpdateNotificationAsync(ndm);
            task.Wait();
            var result = task.Result;
            result.PropertyValuesAreEquals(ndm, new string[] {});

            (result.UpdatedOnUtc != default(DateTime)).ShouldBeTrue();
            ePublisher.Verify(e => e.Publish(It.IsAny<UpdatedEvent<NotificationDomainModel>>()), Times.Exactly(1));
        }

        [Test]
        public void NotificationService_Deletes_Throws()
        {
            var srv = new NotificationService(null, null, null);
            typeof(InvalidOperationException).ShouldBeThrownBy(() => srv.DeleteNotificationAsync(0).RunSynchronously());
            typeof(InvalidOperationException).ShouldBeThrownBy(() => srv.DeleteNotificationAsync(-1).RunSynchronously());
        }

        [Test]
        public void NotificationService_Deletes()
        {
            var repo = new Mock<INotificationRepository>();
            var ePublisher = new Mock<IEventPublisher>();

            var srv = new NotificationService(repo.Object, ePublisher.Object, null);
            srv.DeleteNotificationAsync(123).Wait();
            ePublisher.Verify(e => e.Publish(It.IsAny<DeletedEvent<NotificationDomainModel>>()), Times.Exactly(1));

            //no assertion in the controller as this method is void
        }
    }
}