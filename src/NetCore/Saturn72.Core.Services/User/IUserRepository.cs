using System.Collections.Generic;
using Saturn72.Core.Domain.Identity;


namespace Saturn72.Core.Services.User
{
    public interface IUserRepository
    {
        IEnumerable<UserModel> GetUserByUsername(string username, bool activeOnly=true);
    }
}
