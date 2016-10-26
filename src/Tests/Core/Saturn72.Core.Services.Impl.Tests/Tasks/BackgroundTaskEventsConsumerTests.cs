#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Configuration;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Tasks;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Tasks
{
    public class BackgroundTaskEventsConsumerTests : IDependencyRegistrar
    {
        private static readonly ICollection<BackgroundTaskExecutionDataDomainModel> Items =
            new List<BackgroundTaskExecutionDataDomainModel>();

        private Mock<IBackgroundTaskExecutionDataRepository> _repo;
        public int RegistrationOrder { get; }

        public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
        {
            _repo = new Mock<IBackgroundTaskExecutionDataRepository>();
            _repo.Setup(r => r.CreateTaskExecutionData(It.IsAny<BackgroundTaskExecutionDataDomainModel>()))
                .Callback<BackgroundTaskExecutionDataDomainModel>(t => Items.Add(t));

            return reg =>
            {
                reg.RegisterType<MemoryCacheManager, ICacheManager>(LifeCycle.SingleInstance);

                reg.RegisterInstance(_repo.Object, LifeCycle.SingleInstance);
            };
        }

        [Test]
        public void HandleEvent_ThrowsOnNull()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => new BackgroundTaskEventsConsumer().HandleEvent(null));
            var eventMsg = new CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>(null);

            typeof(NullReferenceException).ShouldBeThrownBy(
                () => new BackgroundTaskEventsConsumer().HandleEvent(eventMsg));
        }

        [Test]
        public void HandleEvent_CreatedEvent_TriggersRepository()
        {
            var model = BuildCreateEventModel();

            var eventMsg = new CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>(model);

            var handler = new BackgroundTaskEventsConsumer();
            handler.HandleEvent(eventMsg);

            Items.Count.ShouldEqual(1);

            Items.First().ShouldEqual(model);
        }

        private static BackgroundTaskExecutionDataDomainModel BuildCreateEventModel()
        {
            var model = new BackgroundTaskExecutionDataDomainModel
            {
                Id = 123,
                CreatedOnUtc = DateTime.UtcNow,
                ErrorData = "This is error data",
                OutputData = "This is outputData",
                ProcessExitCode = 12345678987654321,
                ProcessExitedOnUtc = DateTime.UtcNow,
                ProcessStartedOnUtc = DateTime.UtcNow
            };

            var psi = new ProcessStartInfo
            {
                FileName = @"C:\temp\dummy.bat",
                WorkingDirectory = @"C:\ppp"
            };
            model.SetPocessStartInfo(psi);

            model.SetException(new Exception("This is exception", new NullReferenceException("This is inner exception")));
            return model;
        }
    }
}