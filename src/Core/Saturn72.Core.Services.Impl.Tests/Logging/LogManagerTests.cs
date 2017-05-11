using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Impl.Logging;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Logging
{
    public class LogManagerTests
    {
        [Test]
        public void LogManager_LoadsAllLogLevels()
        {
            var logger1 = new Mock<ILogger>();
            logger1.Setup(l => l.IsEnabled(It.IsAny<LogLevel>()))
                .Returns<LogLevel>(ll => ll == LogLevel.Debug || ll == LogLevel.Error || ll == LogLevel.Information);
            var logger2 = new Mock<ILogger>();
        }

        [Test]
        public void LogManager_LoadsAllLoggersInAppDomain()
        {
            var logManager = new LogManager();
            logManager.AllLoggers.Count().ShouldBe(2);
            logManager.AllLoggers.ElementAt(0).ShouldNotBeNull();
            logManager.AllLoggers.ElementAt(1).ShouldNotBeNull();

        }

        internal class DummyLogger1:ILogger
        {
            public LogLevel[] SupportedLogLevels => new[]
            {
                LogLevel.Debug, LogLevel.Error, LogLevel.Information
            };

            public void DeleteLogRecord(LogRecordModel logRecord)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<LogRecordModel> GetAllLogRecords()
            {
                throw new NotImplementedException();
            }

            public LogRecordModel GetLogById(long logRecordId)
            {
                throw new NotImplementedException();
            }

            public LogRecordModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
                Guid contextId = new Guid())
            {
                throw new NotImplementedException();
            }
        }
        internal class DummyLogger2 : ILogger
        {
            public LogLevel[] SupportedLogLevels => new[]
            {
                LogLevel.Debug ,LogLevel.Error ,LogLevel.Information,LogLevel.Fatal
            };

            public void DeleteLogRecord(LogRecordModel logRecord)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<LogRecordModel> GetAllLogRecords()
            {
                throw new NotImplementedException();
            }

            public LogRecordModel GetLogById(long logRecordId)
            {
                throw new NotImplementedException();
            }

            public LogRecordModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
                Guid contextId = new Guid())
            {
                throw new NotImplementedException();
            }
        }
    }
}