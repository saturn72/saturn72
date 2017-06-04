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
        LogLevel[] SupportedLogLevels { get; }

        /// <summary>
        ///     Deletes a logRecord item
        /// </summary>
        /// <param name="logRecord">Log item</param>
        void DeleteLogRecord(LogRecordModel logRecord);

        /// <summary>
        ///     Gets all logRecord items
        /// </summary>
        /// <value>LogRecord{} <see cref="LogRecord{object}" />
        /// </value>
        IEnumerable<LogRecordModel> AllLogRecords { get; }

        /// <summary>
        ///     Gets a logRecord item
        /// </summary>
        /// <param name="logRecordId"></param>
        /// <returns>Log item</returns>
        LogRecordModel GetLogById(long logRecordId);

        /// <summary>
        ///     Inserts a logRecord item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="contextId">Operation contextId id</param>
        /// <returns>A logRecord item</returns>
        LogRecordModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = default(Guid));
    }
}