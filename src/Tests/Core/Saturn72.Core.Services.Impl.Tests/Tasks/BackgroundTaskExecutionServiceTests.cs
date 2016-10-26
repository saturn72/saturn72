#region

using System;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Tasks;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Tasks
{
    public class BackgroundTaskExecutionServiceTests
    {
        [Test]
        public void CreateTaskExecutionData_ThrowsOnNull()
        {
            var srv = new BackgroundTaskExecutionService(null, null);
            typeof(NullReferenceException).ShouldBeThrownBy(() => srv.CreateTaskExecutionData(null));
        }

        [Test]
        public void CreateTaskExecutionData_CreatesNew()
        {
            var model = new BackgroundTaskExecutionDataDomainModel
            {
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


            var repo = new Mock<IBackgroundTaskExecutionDataRepository>();
            var ePublisher = new Mock<IEventPublisher>();
            var srv = new BackgroundTaskExecutionService(repo.Object, ePublisher.Object);
            srv.CreateTaskExecutionData(model);

            repo.Verify(x => x.CreateTaskExecutionData(It.IsAny<BackgroundTaskExecutionDataDomainModel>()), Times.Once);
            ePublisher.Verify(e => e.Publish(It.IsAny<CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>>()),
                Times.Once);
        }
    }
}