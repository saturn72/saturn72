
using System.Threading.Tasks;
using Saturn72.Core.Domain.Users;

namespace Saturn72.Core.Services.User
{
    public interface IUserActivityLogService
    {
        Task<UserActivityLogModel> AddUserActivityLogAsync(UserActivityType userActivityType, UserModel user);
    }
}
