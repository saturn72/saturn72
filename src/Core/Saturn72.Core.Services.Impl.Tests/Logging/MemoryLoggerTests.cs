using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Services.Impl.Logging;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Logging
{
    public class MemoryLoggerTests
    {
        [Test]
        public void MemoryLogger_SupportedLogLevels()
        {
            var ml = new MemoryLogger();
            ml.SupportedLogLevels.Length.ShouldBe(LogLevel.AllSystemLogLevels.Count());
            ml.SupportedLogLevels.ShouldBeSubsetOf(LogLevel.AllSystemLogLevels);
    }
        [Test]
        public void MemoryLogger_Insert()
        {
            var expected = new LogRecordModel
            {
                ContextId = Guid.NewGuid(),
                FullMessage = "fullMessage",
                ShortMessage = "short_Msge",
                LogLevel = LogLevel.Debug
            };

            var ml = new MemoryLogger();

            var lr = ml.InsertLog(expected.LogLevel, expected.ShortMessage, expected.FullMessage, expected.ContextId);

            lr.Id.ShouldBeGreaterThan(0);
            lr.LogLevel.ShouldBe(expected.LogLevel);
            lr.ShortMessage.ShouldBe(expected.ShortMessage);
            lr.FullMessage.ShouldBe(expected.FullMessage);
            lr.ContextId.ShouldBe(expected.ContextId);
        }

        [Test]
        public void MemoryLogger_GetAllLogRecords()
        {
            var ml = new TestMemoryLogger();
            const int lrCounter = 5;
            const string fullmessage = "fullMessage_";
            const string shortMsge = "short_Msge_";
            ml.LogRecords.Clear();

            var logLevelsCount = LogLevel.AllSystemLogLevels.Count();
            for (var i = 1; i <= lrCounter; i++)
            {
                
                ml.LogRecords.Add(new LogRecordModel
                {
                    Id = i,
                    ContextId = Guid.NewGuid(),
                    FullMessage = fullmessage + i,
                    ShortMessage = shortMsge + i,
                    LogLevel = LogLevel.AllSystemLogLevels.ElementAt(logLevelsCount%i)
                });
            }

            ml.AllLogRecords.Count().ShouldBe(lrCounter);

            foreach (var lr in ml.AllLogRecords)
            {
                var index = lr.Id;
                index.ShouldBeGreaterThanOrEqualTo(1);
                index.ShouldBeLessThanOrEqualTo(index);

                lr.LogLevel.ShouldBe(LogLevel.AllSystemLogLevels.ElementAt(logLevelsCount % (int)index));
                lr.FullMessage.ShouldBe(fullmessage + index);
                lr.ShortMessage.ShouldBe(shortMsge + index);
                lr.ContextId.ShouldNotBe(Guid.Empty);
            }
        }

        [Test]
        public void MemoryLogger_Delete()
        {
            var ml = new TestMemoryLogger();
            const int lrCounter = 5;
            const string fullmessage = "fullMessage_";
            const string shortMsge = "short_Msge_";
            ml.LogRecords.Clear();

            var logLevelsCount = LogLevel.AllSystemLogLevels.Count();
            for (var i = 1; i <= lrCounter; i++)
            {

                ml.LogRecords.Add(new LogRecordModel
                {
                    Id = i,
                    ContextId = Guid.NewGuid(),
                    FullMessage = fullmessage + i,
                    ShortMessage = shortMsge + i,
                    LogLevel = LogLevel.AllSystemLogLevels.ElementAt(logLevelsCount % i)
                });
            }
            ml.LogRecords.Count().ShouldBe(lrCounter);

            //record not existsnot exists
            ml.DeleteLogRecord(new LogRecordModel());
            ml.LogRecords.Count().ShouldBe(lrCounter);

            //record exists
            var lrToRemove = ml.LogRecords.FirstOrDefault();

            ml.DeleteLogRecord(lrToRemove);
            ml.LogRecords.Count().ShouldBe(lrCounter-1);

        }

        [Test]
        public void MemoryLogger_GetLogById()
        {
            var ml = new TestMemoryLogger();
            const int lrCounter = 5;
            const string fullmessage = "fullMessage_";
            const string shortMsge = "short_Msge_";
            ml.LogRecords.Clear();

            var logLevelsCount = LogLevel.AllSystemLogLevels.Count();
            for (var i = 1; i <= lrCounter; i++)
            {

                ml.LogRecords.Add(new LogRecordModel
                {
                    Id = i,
                    ContextId = Guid.NewGuid(),
                    FullMessage = fullmessage + i,
                    ShortMessage = shortMsge + i,
                    LogLevel = LogLevel.AllSystemLogLevels.ElementAt(logLevelsCount % i)
                });
            }

            //not exists
            ml.GetLogById(100).ShouldBeNull();
            //exists
            const int index = 1;
            var lr = ml.GetLogById(index);
            index.ShouldBe(index);
            lr.LogLevel.ShouldBe(LogLevel.AllSystemLogLevels.ElementAt(logLevelsCount % index));
            lr.FullMessage.ShouldBe(fullmessage + index);
            lr.ShortMessage.ShouldBe(shortMsge + index);
            lr.ContextId.ShouldNotBe(Guid.Empty);

        }

        internal class TestMemoryLogger : MemoryLogger
        {
            public new ICollection<LogRecordModel> LogRecords => base.LogRecords;
        }
    }
}
