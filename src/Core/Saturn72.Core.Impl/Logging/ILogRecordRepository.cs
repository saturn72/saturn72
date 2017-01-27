using System.Collections.Generic;
using Saturn72.Core.Domain.Logging;

namespace Saturn72.Core.Services.Impl.Logging
{
    public interface ILogRecordRepository
    {
        LogRecordModel AddLogRecord(LogRecordModel logRecordModel);
        IEnumerable<LogRecordModel> GetAllLogRecords();
        LogRecordModel GetLogRecordById(long logRecordId);
    }
}