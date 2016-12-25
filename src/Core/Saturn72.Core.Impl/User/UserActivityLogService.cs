
using System;
using System.Threading.Tasks;
using Saturn72.Core.Data.Repositories;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.User
{
    public class UserActivityLogService:IUserActivityLogService
    {
        private readonly IWorkContext<long> _workContext;
        private readonly IUserActivityLogRepository _userActivityLogRepository;

        public UserActivityLogService(IUserActivityLogRepository userActivityLogRepository, IWorkContext<long> workContext)
        {
            _workContext = workContext;
            _userActivityLogRepository = userActivityLogRepository;
        }

        public Task<UserActivityLogDomainModel> AddUserActivityLogAsync(UserActivityType userActivityType, UserDomainModel user)
        {
            Guard.NotNull(new object[] { userActivityType, user});
            var ual = new UserActivityLogDomainModel
            {
                ActivityDateUtc = DateTime.UtcNow,
                UserGuid = user.UserGuid,
                ActvityTypeCode = userActivityType.Code,
                ClientApp = _workContext.ClientId,
                UserIpAddress = _workContext.CurrentUserIpAddress
            };
            return Task.FromResult(_userActivityLogRepository.AddUserActivityLog(ual));
        }

    }
}
