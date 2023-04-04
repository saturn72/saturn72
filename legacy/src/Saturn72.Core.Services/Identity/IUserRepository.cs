using Saturn72.Core.Domain.Identity;

namespace Saturn72.Core.Services.Identity
{
    public interface IUserRepository
    {
        UserModel GetByUserUsername(string username, bool activeOnly = true);
    }
}
