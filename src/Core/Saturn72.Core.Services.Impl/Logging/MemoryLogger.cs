using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;

namespace Saturn72.Core.Services.Impl.Logging
{
    public class MemoryLogger : ILogger
    {
        private static long _logRecordIndex;

        private ICollection<LogRecordModel> _logRecords;
        protected ICollection<LogRecordModel> LogRecords => _logRecords ?? (_logRecords = new List<LogRecordModel>());

        public LogLevel[] SupportedLogLevels => LogLevel.AllSystemLogLevels.ToArray();

        public void DeleteLogRecord(LogRecordModel logRecord)
        {
            LogRecords.Remove(logRecord);
        }

        public IEnumerable<LogRecordModel> AllLogRecords
        {
            get { return LogRecords; }
        }

        public LogRecordModel GetLogById(long logRecordId)
        {
            return LogRecords.FirstOrDefault(lr => lr.Id == logRecordId);
        }

        public LogRecordModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = new Guid())
        {
            var lr = new LogRecordModel
            {
                Id = ++_logRecordIndex,
                LogLevel = logLevel,
                FullMessage = fullMessage,
                ShortMessage = shortMessage,
                ContextId = contextId
            };
            LogRecords.Add(lr);
            return lr;
        }
    }
}