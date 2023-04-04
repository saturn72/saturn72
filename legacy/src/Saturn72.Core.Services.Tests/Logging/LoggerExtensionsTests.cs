using Moq;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Services.Logging;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Saturn72.Core.Services.Tests.Logging
{
    public class LoggerExtensionsTests
    {
        [Fact]
        public void LoggerExtensions_IsEnabled()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());
            logger.Object.IsEnabled(LogLevel.Debug).ShouldBeTrue();

            logger.Setup(l => l.SupportedLogLevels).Returns(new LogLevel[] { });
            logger.Object.IsEnabled(LogLevel.Debug).ShouldBeFalse();
        }
        [Fact]
        public void LoggerExtensions_AddLogLevelBasedExtensions()
        {
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());

            var format = "{0} {1} {2}";
            string expMsg = string.Format(format, 1, 2, 3);

            logger.Object.Debug(format, 1, 2, 3);
            logger.Verify(l => l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Debug),
            expMsg,
            It.Is<string>(s => s.Length == 0), default(Guid)), Times.Once);

            logger.Object.Error(expMsg);
            logger.Verify(l => l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Error),
            expMsg,
            It.Is<string>(s => s.Length == 0), default(Guid)), Times.Once);

            logger.Object.Fatal(expMsg);
            logger.Verify(l => l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Fatal),
            expMsg,
            It.Is<string>(s => s.Length == 0), default(Guid)), Times.Once);

            logger.Object.Information(expMsg);
            logger.Verify(l => l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Information),
            expMsg,
            It.Is<string>(s => s.Length == 0), default(Guid)), Times.Once);

            logger.Object.Trace(expMsg);
            logger.Verify(l => l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Trace),
            expMsg,
            It.Is<string>(s => s.Length == 0), default(Guid)), Times.Once);

            logger.Object.Warning(expMsg);
            logger.Verify(l => l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Warning),
            expMsg,
            It.Is<string>(s => s.Length == 0), default(Guid)), Times.Once);

        }
    }
}