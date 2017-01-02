#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Domain.Logging;

#endregion

namespace Saturn72.Core.Logging
{
    /// <summary>
    ///     Represents system logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Determines whether a logRecord level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        ///     Deletes a logRecord item
        /// </summary>
        /// <param name="logRecord">Log item</param>
        void DeleteLog(LogRecordDomainModel logRecord);

        /// <summary>
        ///     Gets all logRecord items
        /// </summary>
        /// <returns>LogRecord{} <see cref="LogRecord{object}" /></returns>
        IEnumerable<LogRecordDomainModel> GetAllLogRecords();

        /// <summary>
        ///     Gets a logRecord item
        /// </summary>
        /// <param name="logRecordId"></param>
        /// <returns>Log item</returns>
        LogRecordDomainModel GetLogById(long logRecordId);

        /// <summary>
        ///     Inserts a logRecord item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="contextId">Operation contextId id</param>
        /// <returns>A logRecord item</returns>
        LogRecordDomainModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = default(Guid));
    }
}