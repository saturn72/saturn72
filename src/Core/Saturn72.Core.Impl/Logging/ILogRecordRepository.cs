using System.Collections.Generic;
using Saturn72.Core.Domain.Logging;

namespace Saturn72.Core.Services.Impl.Logging
{
    public interface ILogRecordRepository
    {
        LogRecordDomainModel AddLogRecord(LogRecordDomainModel logRecord);
        IEnumerable<LogRecordDomainModel> GetAllLogRecords();
        LogRecordDomainModel GetLogRecordById(long logRecordId);
    }
}