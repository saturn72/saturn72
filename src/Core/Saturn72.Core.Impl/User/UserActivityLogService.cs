﻿using System;
using System.Threading.Tasks;
using Saturn72.Core.Data.Repositories;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.User
{
    public class UserActivityLogService : IUserActivityLogService
    {
        private readonly IUserActivityLogRepository _userActivityLogRepository;

        public UserActivityLogService(IUserActivityLogRepository userActivityLogRepository)
        {
            _userActivityLogRepository = userActivityLogRepository;
        }

        public Task<UserActivityLogDomainModel> AddUserActivityLogAsync(UserActivityType userActivityType,
            UserDomainModel user)
        {
            Guard.NotNull(new object[] {userActivityType, user});
            var ual = new UserActivityLogDomainModel
            {
                ActivityDateUtc = DateTime.UtcNow,
                UserId = user.Id,
                ActivityTypeCode = userActivityType.Code,
                ClientAppId = user.LastClientAppId,
                UserIpAddress = user.LastIpAddress
            };
            return Task.FromResult(_userActivityLogRepository.AddUserActivityLog(ual));
        }
    }
}