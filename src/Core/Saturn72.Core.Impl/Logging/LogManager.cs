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
        private LoggerCollection _loggerCollection;

        private LoggerCollection LoggerCollection => _loggerCollection ?? (_loggerCollection = GetAllAppDomainLoggers())
            ;

        public IEnumerable<ILogger> AllLoggers
        {
            get { return LoggerCollection.Loggers; }
        }

        public LogLevel[] SupportedLogLevels { get; private set; }

        public void DeleteLogRecord(LogRecordDomainModel logRecord)
        {
            LoggerCollection[logRecord.LogLevel].DeleteLogRecord(logRecord);
        }

        public IEnumerable<LogRecordDomainModel> GetAllLogRecords()
        {
            var result = new List<LogRecordDomainModel>();
            AllLoggers.ForEachItem(l => result.AddRange(l.GetAllLogRecords()));
            return result;
        }

        public LogRecordDomainModel GetLogById(long logRecordId)
        {
            throw new NotSupportedException();
        }

        public LogRecordDomainModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = new Guid())
        {
            LoggerCollection[logLevel].ForEachItem(l =>
                Task.Run(() => l.InsertLog(logLevel, shortMessage, fullMessage, contextId)));

            throw new NotImplementedException();
        }

        private LogLevel[] GetSupportedLevels()
        {
            var result = LoggerCollection.Loggers.SelectMany(l => l.SupportedLogLevels).Distinct();
            return result.ToArray();
        }

        #region Utiltities

        private LoggerCollection GetAllAppDomainLoggers()
        {
            var allLoggers = AppEngine.Current.ResolveAll<ILogger>()
                .Where(t => t.GetType() != GetType());

            var loggersDictionary = BuildLoggersDictionary(allLoggers);
            var result = new LoggerCollection(loggersDictionary);
            return result;
        }

        private IDictionary<LogLevel, IEnumerable<ILogger>> BuildLoggersDictionary(IEnumerable<ILogger> loggers)
        {
            SupportedLogLevels = loggers.SelectMany(l => l.SupportedLogLevels).Distinct().ToArray();
            var result = new Dictionary<LogLevel, IEnumerable<ILogger>>();
            SupportedLogLevels.ForEachItem(ll => result[ll] = new List<ILogger>());

            loggers.ForEachItem(logger =>
            {
                foreach (var logLevel in SupportedLogLevels)
                {
                    if (logger.IsEnabled(logLevel))
                        (result[logLevel] as IList<ILogger>).Add(logger);
                }
            });
            return result;
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