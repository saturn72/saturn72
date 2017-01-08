#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public class UserService : DomainModelCrudServiceBase<UserDomainModel, long, long>, IUserService
    {
        private const string UserRolesUserCacheKey = "Saturn72_User{0}_UserRoles";

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IEventPublisher eventPublisher, ICacheManager cacheManager,
            ITypeFinder typeFinder, IWorkContext<long> workContext )
            : base(eventPublisher, cacheManager, typeFinder, workContext)
        {
            _userRepository = userRepository;
        }

        public UserDomainModel GetUserByUsername(string username)
        {
            Guard.HasValue(username);
            return GetUserBy(u => u.Active && u.Username == username);
        }

        public UserDomainModel GetUserByEmail(string email)
        {
            Guard.HasValue(email);
            Guard.MustFollow(CommonHelper.IsValidEmail(email), "invalid email address");
            return GetUserBy(u => u.Active && u.Email == email);
        }

        public UserDomainModel GetUserBy(Func<UserDomainModel, bool> func)
        {
            var users = FilterTable(func);
            Guard.MustFollow(() => users.Count() <= 1);

            return users.FirstOrDefault();
        }

        public Task<IEnumerable<UserRoleDomainModel>> GetUserUserRolesByUserIdAsync(long userId)
        {
            Guard.GreaterThan(userId, (long) 0);

            return Task.FromResult(
                CacheManager.Get(UserRolesUserCacheKey, () =>
                {
                    var res = _userRepository.GetUserUserRoles(userId);
                    //User must have user roles
                    Guard.NotNull(res);

                    return res;
                }));
        }

        public void UpdateUser(UserDomainModel user)
        {
            Update(user);
        }
    }
}