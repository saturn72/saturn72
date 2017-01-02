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
            new DefaultLogger(null).IsEnabled(null).ShouldBeTrue();
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
            lrRepo.Setup(l => l.AddLogRecord(It.IsAny<LogRecordDomainModel>()))
                .Returns<LogRecordDomainModel>(l =>
                {
                    l.Id = 100;
                    return l;
                });

            var logger = new DefaultLogger(lrRepo.Object);
            var expected = new LogRecordDomainModel
            {
                LogLevel = LogLevel.Debug,
                ShortMessage = "shortMessage",
                FullMessage = "Full Message",
            };

            logger.InsertLog(expected.LogLevel, expected.ShortMessage, expected.FullMessage).PropertyValuesAreEquals(expected,new [] {"Id", "ContextId"});
        }

        [Test]
        public void DefaultLogger_GetAllLogRecords_ReturndLogRecordCollection()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            var expected = new[]
            {
                new LogRecordDomainModel(),
                new LogRecordDomainModel(),
                new LogRecordDomainModel(),
                new LogRecordDomainModel(),
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
                .Returns((LogRecordDomainModel) null);
            var logger = new DefaultLogger(lrRepo.Object);
            logger.GetLogById(10).ShouldBeNull();
        }

    }
}