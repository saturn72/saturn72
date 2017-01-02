#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace Saturn72.Core.Logging
{
    public static class LoggerCollectionExtensions
    {
        public static async void Debug(this IEnumerable<ILogger> loggers, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            await IterateLoggersAsync(loggers, logger => logger.Debug(message, exception));
        }


        public static async void Information(this IEnumerable<ILogger> loggers, string message,
            Exception exception = null,
            Guid contextId = default(Guid))
        {
            await IterateLoggersAsync(loggers, logger => logger.Information(message, exception));
        }

        public static async void Warning(this IEnumerable<ILogger> loggers, string message,
            Exception exception = null,
            Guid contextId = default(Guid))
        {
            await IterateLoggersAsync(loggers, logger => logger.Warning(message, exception));
        }

        public static async void Error(this IEnumerable<ILogger> loggers, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            await IterateLoggersAsync(loggers, logger => logger.Error(message, exception));
        }

        public static async void Fatal(this IEnumerable<ILogger> loggers, string message, Exception exception = null,
            Guid contextId = default(Guid))
        {
            await IterateLoggersAsync(loggers, logger => logger.Fatal(message, exception));
        }

        private static async Task IterateLoggersAsync(IEnumerable<ILogger> loggers, Action<ILogger> action)
        {
            foreach (var l in loggers)
                await Task.Run(() => action(l));
        }
    }
}