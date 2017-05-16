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
            var loggers = new ILogger[] { new DummyLogger1(), new DummyLogger2() };
            var lm = new LogManager(loggers);
            lm.SupportedLogLevels.Length.ShouldBe(4);
            lm.SupportedLogLevels.ShouldContain(LogLevel.Debug);
            lm.SupportedLogLevels.ShouldContain(LogLevel.Error);
            lm.SupportedLogLevels.ShouldContain(LogLevel.Information);
            lm.SupportedLogLevels.ShouldContain(LogLevel.Fatal);

        }

        [Test]
        public void LogManager_LoadsAllLoggersInAppDomain()
        {
            var loggers = new ILogger[] {new DummyLogger1(), new DummyLogger2()};
            var allLoggers = new LogManager(loggers).AllLoggers;
            allLoggers.Count().ShouldBe(2);
            allLoggers.ElementAt(0).ShouldNotBeNull();
            allLoggers.ElementAt(1).ShouldNotBeNull();

        }
    }

    public class DummyLogger1 : ILogger
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

    public class DummyLogger2 : ILogger
    {
        public LogLevel[] SupportedLogLevels => new[]
        {
            LogLevel.Debug, LogLevel.Error, LogLevel.Information, LogLevel.Fatal
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