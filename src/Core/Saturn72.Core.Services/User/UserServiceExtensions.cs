using System.Linq;
using Saturn72.Core.Domain.Users;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.User
{
    public static class UserServiceExtensions
    {
        public static void LoadUserRoles(this IUserService userService, UserDomainModel user)
        {
            Guard.NotNull(user);

            var userRoles = userService.GetUserUserRolesByUserId(user.Id);
            userRoles.Where(ur => !user.UserRoles.Contains(ur)).ForEachItem(user.UserRoles.Add);
        }
    }
}