﻿using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.Impl.User
{
    public interface IUserActivityLogRepository
    {
        UserActivityLogDomainModel AddUserActivityLog(UserActivityLogDomainModel userActivityLog);
    }
}