using Saturn72.Core.Domain.ActivityLog;

namespace Saturn72.Core.Services.ActivityLog
{
    public interface IActivityLogRecordRepository
    {
        void Add(ActivityLogRecord logRecord);
    }
}
