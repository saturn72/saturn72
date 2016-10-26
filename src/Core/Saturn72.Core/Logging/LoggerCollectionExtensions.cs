#region

using System;
using System.Collections.Generic;
using System.Threading;
using Saturn72.Extensions;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Logging
{
    public static class LoggerCollectionExtensions
    {
        public static void Debug(this IEnumerable<ILogger> loggers, string message, Exception exception = null, Guid contextId = default(Guid))
        {
            IterateLoggers(loggers, logger => logger.Debug(message, exception));
        }


        public static void Information(this IEnumerable<ILogger> loggers, string message,
            Exception exception = null,
            Guid contextId = default(Guid))
        {
            IterateLoggers(loggers, logger => logger.Information(message, exception));
        }

        public static void Warning(this IEnumerable<ILogger> loggers, string message,
            Exception exception = null,
            Guid contextId = default(Guid))
        {
            IterateLoggers(loggers, logger => logger.Warning(message, exception));
        }

        public static void Error(this IEnumerable<ILogger> loggers, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            IterateLoggers(loggers, logger => logger.Error(message, exception));
        }

        public static void Fatal(this IEnumerable<ILogger> loggers, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            IterateLoggers(loggers, logger => logger.Fatal(message, exception));
        }

        private static void FilteredLog(ILogger logger, LogLevel level, string message,
            Exception exception = null, Guid contextId = default(Guid))
        {
            //don't log thread abort exception
            if (exception is ThreadAbortException || !logger.IsEnabled(level))
                return;

            var fullMessage = exception.NotNull() ? exception.ToString() : string.Empty;
            logger.InsertLog(level, message, fullMessage, contextId);
        }

        private static void IterateLoggers(IEnumerable<ILogger> loggers, Action<ILogger> action)
        {
            foreach (var l in loggers)
                action(l);
        }
    }
}