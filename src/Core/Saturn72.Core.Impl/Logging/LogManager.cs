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
    public class LoggerCollection
    {
        private readonly IDictionary<LogLevel, IEnumerable<ILogger>> _allLoggersDictionary;
        private IEnumerable<ILogger> _loggers;

        public LoggerCollection(IDictionary<LogLevel, IEnumerable<ILogger>> allLoggersDictionary)
        {
            _allLoggersDictionary = allLoggersDictionary;
        }

        public IEnumerable<ILogger> Loggers
        {
            get { return _loggers ?? (_loggers = _allLoggersDictionary.Values.SelectMany(s => s).Distinct()); }
        }

        public IEnumerable<ILogger> this[LogLevel logLevel] => _allLoggersDictionary[logLevel];
    }

    public class LogManager : ILogger
    {
        private LoggerCollection _loggerCollection;

        public LoggerCollection AllLoggers
        {
            get { return _loggerCollection ?? (_loggerCollection = GetAllAppDomainLoggers()); }
        }

        public bool IsEnabled(LogLevel level)
        {
            return true;
        }

        public void DeleteLogRecord(LogRecordDomainModel logRecord)
        {
            AllLoggers[logRecord.LogLevel].DeleteLogRecord(logRecord);
        }

        public IEnumerable<LogRecordDomainModel> GetAllLogRecords()
        {
            var result = new List<LogRecordDomainModel>();
            AllLoggers.Loggers.ForEachItem(l => result.AddRange(l.GetAllLogRecords()));
            return result;
        }

        public LogRecordDomainModel GetLogById(long logRecordId)
        {
            throw new NotSupportedException();
        }

        public LogRecordDomainModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = new Guid())
        {
            AllLoggers[logLevel].ForEachItem(l =>
                Task.Run(() => l.InsertLog(logLevel, shortMessage, fullMessage, contextId)));

            throw new NotImplementedException();
        }

        #region Utiltities

        private LoggerCollection GetAllAppDomainLoggers()
        {
            var allLoggers = AppEngine.Current.Resolve<ITypeFinder>().FindClassesOfType<ILogger>()
                .Where(t => t != GetType())
                .Select(t => AppEngine.Current.TryResolve<ILogger>(t));

            var loggersDictionary = BuildLoggersDictionary(allLoggers);
            var result = new LoggerCollection(loggersDictionary);
            return result;
        }

        private IDictionary<LogLevel, IEnumerable<ILogger>> BuildLoggersDictionary(IEnumerable<ILogger> loggers)
        {
            var allLogLevel = GetAllAppDomainLogLevels();

            var result = new Dictionary<LogLevel, IEnumerable<ILogger>>();
            allLogLevel.ForEachItem(ll => result[ll] = new List<ILogger>());

            loggers.ForEachItem(logger =>
            {
                foreach (var logLevel in allLogLevel)
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