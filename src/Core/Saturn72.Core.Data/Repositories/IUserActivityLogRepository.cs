using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Data.Repositories
{
    public interface IUserActivityLogRepository
    {
        UserActivityLogDomainModel AddUserActivityLog(UserActivityLogDomainModel userActivityLog);
    }
}
