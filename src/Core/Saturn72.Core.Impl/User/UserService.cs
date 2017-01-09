#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public class UserService : IUserService
    {
        private const string UserRolesUserCacheKey = "Saturn72_User{0}_UserRoles";
        private const string UserCacheKey = "saturn72. User-{0}";
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        private readonly IUserRepository _userRepository;
        private readonly AuditHelper _auditHelper;

        public UserService(IUserRepository userRepository, IEventPublisher eventPublisher, ICacheManager cacheManager, AuditHelper auditHelper)
        {
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
            _auditHelper = auditHelper;
        }

        public Task<IEnumerable<UserDomainModel>> GetAllUsersAsync()
        {
            return
                Task.Run(() => _cacheManager.Get(SystemSharedCacheKeys.AllUserCacheKey, () => _userRepository.GetAll()));
        }

        public async Task<UserDomainModel> GetUserByUsername(string username)
        {
            Guard.HasValue(username);
            return await GetUserByFuncAndCacheIfExists(u => u.Active && u.Username == username);
        }

        public async Task<UserDomainModel> GetUserByEmail(string email)
        {
            Guard.HasValue(email);
            Guard.MustFollow(CommonHelper.IsValidEmail(email), "invalid email address");
            return await GetUserBy(user => user.Email.EqualsTo(email));
        }

        public Task<IEnumerable<UserRoleDomainModel>> GetUserUserRolesByUserIdAsync(long userId)
        {
            Guard.GreaterThan(userId, (long) 0);

            return Task.FromResult(
                _cacheManager.Get(UserRolesUserCacheKey, () =>
                {
                    var res = _userRepository.GetUserUserRoles(userId);
                    //User must have user roles
                    Guard.NotNull(res);
                    Guard.NotEmpty(res, ()=>
                    {
                        throw new NullReferenceException("userroles");
                    });

                    return res;
                }));
        }

        public async Task UpdateUser(UserDomainModel user)
        {
            Guard.NotNull(user);
            _auditHelper.PrepareForUpdateAudity(user);
            await Task.Run(() => _userRepository.Update(user));
            _cacheManager.RemoveByPattern(SystemSharedCacheKeys.AllUserCacheKey);
            _eventPublisher.DomainModelUpdated(user);
        }

        protected virtual async Task<UserDomainModel> GetUserByFuncAndCacheIfExists(Func<UserDomainModel, bool> func)
        {
            Guard.NotNull(func);
            var allUsers = await GetAllUsersAsync();
            var user = allUsers.FirstOrDefault(func);
            if (user.NotNull())
                _cacheManager.SetIfNotExists(UserCacheKey.AsFormat(user.Id), user);
            return user;
        }

        public async Task<UserDomainModel> GetUserBy(Func<UserDomainModel, bool> func)
        {
            var users = await GetAllUsersAsync();
            return users.FirstOrDefault(func);
        }
    }
}