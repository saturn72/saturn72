#region

using System;
using System.Threading;
using Saturn72.Core.Domain.Logging;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Logging
{
    public static class LoggerExtensions
    {
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