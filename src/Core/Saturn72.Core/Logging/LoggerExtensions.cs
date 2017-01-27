#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Saturn72.Core.Domain.Logging;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Logging
{
    public static class LoggerExtensions
    {

        /// <summary>
        ///     Determines whether a logRecord logLevel is enabled
        /// </summary>
        /// <param name="logLevel">Log logLevel</param>
        /// <returns>Result</returns>
        public static bool IsEnabled(this ILogger logger, LogLevel logLevel)
        {
            return logger.SupportedLogLevels.Contains(logLevel);
        }
        /// <summary>
        ///     Get logRecord items by identifiers
        /// </summary>
        /// <param name="logger">the logger</param>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        public static IEnumerable<LogRecordModel> GetLogByIds(this ILogger logger, long[] logIds)
        {
            var result = new List<LogRecordModel>();
            logIds.ForEachItem(li =>
            {
                var lr = logger.GetLogById(li);
                if (lr.IsNull())
                    return;
                result.Add(lr);
            });
            return result;
        }


        /// <summary>
        ///     Deletes all log records
        /// </summary>
        public static void ClearLog(this ILogger logger)
        {
            logger.GetAllLogRecords().ForEachItem(logger.DeleteLogRecord);
        }


        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            FilteredLog(logger, LogLevel.Debug, string.Format(format, args));
        }

        public static void Debug(this ILogger logger, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            FilteredLog(logger, LogLevel.Debug, message, exception);
        }

        public static void Information(this ILogger logger, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            FilteredLog(logger, LogLevel.Information, message, exception);
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            FilteredLog(logger, LogLevel.Warning, message, exception);
        }

        public static void Error(this ILogger logger, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            FilteredLog(logger, LogLevel.Error, message, exception);
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            FilteredLog(logger, LogLevel.Fatal, message, exception);
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
    }
}