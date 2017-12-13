#region

using Saturn72.Core.Domain.Logging;
using System;
using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Services.Logging
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
        /// <param name="logRecordId"></param>
        void DeleteLogRecord(long logRecordId);

        /// <summary>
        ///     Gets all logRecord items
        /// </summary>
        /// <returns>LogRecord{} <see cref="LogRecordModel{object}" /></returns>
        IEnumerable<LogRecordModel> GetAllLogRecords();

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