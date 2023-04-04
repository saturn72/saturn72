using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Domain.Logging;

namespace Saturn72.Core.Services.Logging
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
}