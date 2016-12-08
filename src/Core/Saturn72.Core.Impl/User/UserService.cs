#region

using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Caching;
using Saturn72.Core.Data.Repositories;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public class UserService : DomainModelCrudServiceBase<UserDomainModel, long>, IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IEventPublisher eventPublisher, ICacheManager cacheManager, ITypeFinder typeFinder)
            : base(userRepository, eventPublisher, cacheManager, typeFinder)
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
            Guard.MustFollow(CommonHelper.IsValidEmail(email));
            return GetUserBy(u => u.Active && u.Email == email);
        }

        public UserDomainModel GetUserBy(Func<UserDomainModel, bool> func)
        {
            var users = FilterTable(func);
            Guard.MustFollow(() => users.Count() <= 1);

            return users.FirstOrDefault();
        }

        public IEnumerable<UserRoleDomainModel> GetUserUserRolesByUserId(long userId)
        {
            return _userRepository.GetUserUserRoles(userId);
        }

        public void UpdateUser(UserDomainModel user)
        {
            Update(user);
        }
    }
}