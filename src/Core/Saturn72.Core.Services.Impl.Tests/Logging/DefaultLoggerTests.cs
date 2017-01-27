using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Services.Impl.Logging;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.Logging
{
    public class DefaultLoggerTests
    {
        [Test]
        public void DefaultLogger_IEnabled_ReturnsTrue()
        {
            var logger = new DefaultLogger(null);
            logger.SupportedLogLevels.Length.ShouldEqual(LogLevel.AllSystemLogLevels.Count());

            logger.SupportedLogLevels.Any(ll => ll.Code == LogLevel.Debug.Code).ShouldBeTrue();
            logger.SupportedLogLevels.Any(ll => ll.Code == LogLevel.Error.Code).ShouldBeTrue();
            logger.SupportedLogLevels.Any(ll => ll.Code == LogLevel.Fatal.Code).ShouldBeTrue();
            logger.SupportedLogLevels.Any(ll => ll.Code == LogLevel.Information.Code).ShouldBeTrue();
            logger.SupportedLogLevels.Any(ll => ll.Code == LogLevel.Trace.Code).ShouldBeTrue();
            logger.SupportedLogLevels.Any(ll => ll.Code == LogLevel.Warning.Code).ShouldBeTrue();
        }

        [Test]
        public void DefaultLogger_InsertLog_Throws()
        {
            var logger = new DefaultLogger(null);
            //on null loglevel
            typeof(NullReferenceException).ShouldBeThrownBy(() => logger.InsertLog(null, null));
            typeof(ArgumentException).ShouldBeThrownBy(() => logger.InsertLog(LogLevel.Debug, null));
            typeof(ArgumentException).ShouldBeThrownBy(() => logger.InsertLog(LogLevel.Debug, ""));
        }

        [Test]
        public void DefaultLogger_InsertLog()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            lrRepo.Setup(l => l.AddLogRecord(It.IsAny<LogRecordModel>()))
                .Returns<LogRecordModel>(l =>
                {
                    l.Id = 100;
                    return l;
                });

            var logger = new DefaultLogger(lrRepo.Object);
            var expected = new LogRecordModel
            {
                LogLevel = LogLevel.Debug,
                ShortMessage = "shortMessage",
                FullMessage = "Full Message"
            };

            logger.InsertLog(expected.LogLevel, expected.ShortMessage, expected.FullMessage)
                .PropertyValuesAreEquals(expected, new[] {"Id", "ContextId"});
        }

        [Test]
        public void DefaultLogger_GetAllLogRecords_ReturndLogRecordCollection()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            var expected = new[]
            {
                new LogRecordModel(),
                new LogRecordModel(),
                new LogRecordModel(),
                new LogRecordModel()
            };
            lrRepo.Setup(l => l.GetAllLogRecords())
                .Returns(expected);
            var logger = new DefaultLogger(lrRepo.Object);

            logger.GetAllLogRecords().Count().ShouldEqual(expected.Length);
        }

        [Test]
        public void DefaultLogger_GetLogRecordById_Throws()
        {
            var logger = new DefaultLogger(null);
            //on illegal id
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => logger.GetLogById(0));
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => logger.GetLogById(-123));
        }

        [Test]
        public void DefaultLogger_GetLogRecordById_ReturnsNullOnNotExistId()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            lrRepo.Setup(l => l.GetLogRecordById(It.IsAny<long>()))
                .Returns((LogRecordModel) null);
            var logger = new DefaultLogger(lrRepo.Object);
            logger.GetLogById(10).ShouldBeNull();
        }
    }
}