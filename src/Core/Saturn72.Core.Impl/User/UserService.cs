#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public class UserService : IUserService
    {
        private readonly AuditHelper _auditHelper;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        private readonly IUserRepository _userRepository;
        private ILogger _logger;

        public UserService(IUserRepository userRepository, IEventPublisher eventPublisher, ICacheManager cacheManager,
            AuditHelper auditHelper, ILogger logger)
        {
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
            _auditHelper = auditHelper;
        }

        public Task<IEnumerable<UserModel>> GetAllUsersAsync(Func<UserModel, bool> filter = null)
        {
            if (filter.IsNull())
                filter = u => u.Active;
            return Task.Run(() => _userRepository.GetBy(filter));
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            Guard.HasValue(username);
            Func<UserModel, bool> filter = u => u.Active && u.Username == username;

            var allMatchingUsers = await GetAllUsersAsync(filter);

            if (allMatchingUsers.IsNull())
                return null;

            if (allMatchingUsers.Count() > 1)
                _logger.Error("Found more than sinlg eactive users with the same username. Username = {0}.".AsFormat(username));

            allMatchingUsers.ForEachItem(
                u => _cacheManager.Set(SystemSharedCacheKeys.UserByIdCacheKey.AsFormat(u.Id), u));
            return allMatchingUsers.OrderByDescending(u => u.Id).FirstOrDefault();
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            Guard.HasValue(email);
            Guard.MustFollow(CommonHelper.IsValidEmail(email), "invalid email address");
            Func<UserModel, bool> filter = u => u.Active && u.Email.EqualsTo(email);

            var allMatchingUsers = await GetAllUsersAsync(filter);

            if (allMatchingUsers.IsNull())
                return null;

            if (allMatchingUsers.Count() > 1)
                _logger.Error("Found more than sinlge active users with the same email address. Email address = {0}.".AsFormat(email));

            allMatchingUsers.ForEachItem(
                u => _cacheManager.Set(SystemSharedCacheKeys.UserByIdCacheKey.AsFormat(u.Id), u));
            return allMatchingUsers.OrderByDescending(u => u.Id).FirstOrDefault();
        }

        public async Task<IEnumerable<UserRoleModel>> GetUserUserRolesByUserIdAsync(long userId)
        {
            Guard.GreaterThan(userId, (long) 0);

            return await Task.FromResult(
                _cacheManager.Get(SystemSharedCacheKeys.UserRolesUserCacheKey.AsFormat(userId),
                    () => _userRepository.GetUserUserRoles(userId) ?? new UserRoleModel[] {}));
        }

        public async Task UpdateUser(UserModel user)
        {
            Guard.NotNull(user);
            _auditHelper.PrepareForUpdateAudity(user);
            await Task.Run(() => _userRepository.Update(user));
            _cacheManager.RemoveByPattern(SystemSharedCacheKeys.UserPatternCacheKey);
            _eventPublisher.DomainModelUpdated(user);
        }

        public async Task<IEnumerable<PermissionRecordModel>> GetUserPermissionsAsync(long userId)
        {
            Guard.GreaterThan(userId, (long) 0);

            return await Task.Run(() => _userRepository.GetUserPermissions(userId));
        }
    }
}