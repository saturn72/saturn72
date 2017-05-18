using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Logging;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.Logging
{
    public class LogManager : ILogger
    {
        private readonly LoggerCollection _loggerCollection;

        public LogManager(IEnumerable<ILogger> loggers)
        {
            _loggerCollection = ToLoggerCollection(loggers);
        }

        public IEnumerable<ILogger> AllLoggers => _loggerCollection.Loggers;

        public LogLevel[] SupportedLogLevels { get; private set; }

        public void DeleteLogRecord(LogRecordModel logRecord)
        {
            _loggerCollection[logRecord.LogLevel].DeleteLogRecord(logRecord);
        }

        public IEnumerable<LogRecordModel> GetAllLogRecords()
        {
            var result = new List<LogRecordModel>();
            AllLoggers.ForEachItem(l => result.AddRange(l.GetAllLogRecords()));
            return result;
        }

        public LogRecordModel GetLogById(long logRecordId)
        {
            throw new NotSupportedException();
        }

        public LogRecordModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = new Guid())
        {
            _loggerCollection[logLevel]
                .ForEachItem(l =>
                    Task.Run(() => l.InsertLog(logLevel, shortMessage, fullMessage, contextId)));

            throw new NotImplementedException();
        }

        private LogLevel[] GetSupportedLevels()
        {
            var result = _loggerCollection.Loggers.SelectMany(l => l.SupportedLogLevels).Distinct();
            return result.ToArray();
        }

        #region Utiltities

        private LoggerCollection ToLoggerCollection(IEnumerable<ILogger> loggers)
        {
            SupportedLogLevels = loggers.SelectMany(l => l.SupportedLogLevels).Distinct().ToArray();

            var result = new Dictionary<LogLevel, IEnumerable<ILogger>>();
            SupportedLogLevels.ForEachItem(ll => result[ll] = new List<ILogger>());

            loggers.ForEachItem(logger =>
            {
                foreach (var logLevel in SupportedLogLevels)
                    if (logger.IsEnabled(logLevel))
                        (result[logLevel] as IList<ILogger>).Add(logger);
            });

            return new LoggerCollection(result);
        }

        private IEnumerable<LogLevel> GetAllAppDomainLogLevels()
        {
            var allLogLevel = new List<LogLevel>();
            var typeFinder = AppEngine.Current.Resolve<ITypeFinder>();
            typeFinder.FindClassesOfTypeAndRunMethod<LogLevel>(allLogLevel.Add);
            return allLogLevel;
        }

        #endregion Utilties
    }
}