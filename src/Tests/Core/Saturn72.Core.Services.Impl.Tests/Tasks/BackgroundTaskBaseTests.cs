#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Configuration;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Tasks;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Tasks
{
    public class BackgroundTaskBaseTests : IDependencyRegistrar
    {
        public static Mock<IEventPublisher> _eventPublisher;
        private static BackgroundTaskExecutionDataDomainModel model;

        public int RegistrationOrder { get; }

        public Action<IIocRegistrator> RegistrationLogic(ITypeFinder typeFinder, Saturn72Config config)
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _eventPublisher.Setup(x => x.Publish(It.IsAny<CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>>()))
                .Callback<CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>>(i => model = i.DomainModel);
            return reg => reg.RegisterInstance(_eventPublisher.Object, LifeCycle.SingleInstance);
        }

        [Test]
        [Ignore("failed test")]
        [NUnit.Framework.Category("ignored")]
        public void Executes_Throws()
        {
            //no event publishing here since tha task was not start execution
            var task1 = new NotImplementedStartInfoBgTask();
            typeof(NotImplementedException).ShouldBeThrownBy(() => task1.Execute());

            var task2 = new NotExistsProcessBgTask();
            typeof(InvalidOperationException).ShouldBeThrownBy(() => task2.Execute());
            _eventPublisher.Verify(
                e => e.Publish(It.IsAny<CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>>()),
                Times.Once);

            model = null;
            var task3 = new ExceptionFromProcessBgTask();
            typeof(Win32Exception).ShouldBeThrownBy(() => task3.Execute());
            _eventPublisher.Verify(
                e => e.Publish(It.IsAny<CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>>()),
                Times.Exactly(2));
            model.ProcessExitCode.ShouldNotEqual(0);
            model.ErrorData.HasValue().ShouldBeTrue();
            model.OutputData.HasValue().ShouldBeTrue();
            model.ProcessStartInfo.HasValue().ShouldBeTrue();
            model.ProcessExitedOnUtc.ShouldNotBeNull();
            model.ProcessId.ShouldNotEqual(0);
        }
    }

    public class ExceptionFromProcessBgTask : BackgroundTaskBase
    {
        public override ProcessStartInfo GetProcessStartInfo()
        {
            var curDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            return new ProcessStartInfo
            {
                FileName = Path.Combine(curDir, "resources\\ThrowsException.exe")
            };
        }
    }

    public class NotExistsProcessBgTask : BackgroundTaskBase
    {
        public override ProcessStartInfo GetProcessStartInfo()
        {
            return new ProcessStartInfo();
        }
    }

    public class NotImplementedStartInfoBgTask : BackgroundTaskBase
    {
        public override ProcessStartInfo GetProcessStartInfo()
        {
            throw new NotImplementedException();
        }
    }
}