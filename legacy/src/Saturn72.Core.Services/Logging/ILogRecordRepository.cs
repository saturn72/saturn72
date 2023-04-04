using System.Collections.Generic;
using Saturn72.Core.Domain.Logging;

namespace Saturn72.Core.Services.Logging
{
    public interface ILogRecordRepository
    {
        void Delete(long logRecordId);
        IEnumerable<LogRecordModel> GetAll();
        LogRecordModel GetById(long logRecordId);
        long Create(LogRecordModel logRecordModel);
    }
}