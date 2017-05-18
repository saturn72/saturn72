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
using Saturn72.Core.Services.Impl.Security;
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
        private readonly ILogger _logger;
        private readonly IPermissionRecordRepository _permissionRecordRepository;

        public UserService(IUserRepository userRepository, IEventPublisher eventPublisher, ICacheManager cacheManager,
            AuditHelper auditHelper, ILogger logger, IPermissionRecordRepository permissionRecordRepository)
        {
            _userRepository = userRepository;
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
            _auditHelper = auditHelper;
            _logger = logger;
            _permissionRecordRepository = permissionRecordRepository;
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            Guard.HasValue(username);
            var allMatchingUsers =
                await Task.FromResult(_userRepository.GetUsersByUsername(username)?.Where(u => !u.Deleted));

            if (allMatchingUsers.IsEmptyOrNull())
                return null;

            if (allMatchingUsers.Count() > 1)
                _logger.Error("Found more than single active users with the same username. Username = {0}.".AsFormat(username));

            allMatchingUsers.ForEachItem(
                u => _cacheManager.Set(SystemSharedCacheKeys.UserByIdCacheKey.AsFormat(u.Id), u));
            return allMatchingUsers.OrderByDescending(u => u.Id).FirstOrDefault();
        }

        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            Guard.HasValue(email);
            Guard.MustFollow(CommonHelper.IsValidEmail(email), "invalid email address");

            var allMatchingUsers = await Task.FromResult(_userRepository.GetUsersByEmail(email)?.Where(u => !u.Deleted));
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
                    () => _permissionRecordRepository.GetUserUserRoles(userId) ?? new UserRoleModel[] {}));
        }

        public async Task UpdateUserAsync(UserModel user)
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

            return await Task.Run(() => _permissionRecordRepository.GetUserPermissions(userId));
        }
    }
}