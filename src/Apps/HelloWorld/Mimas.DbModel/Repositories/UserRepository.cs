#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Common.Data.Repositories;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Data.Repositories;

#endregion

namespace Mimas.DbModel.Repositories
{
    public class UserRepository : RepositoryBase<UserDomainModel, long, User>, IUserRepository
    {
        public UserRepository(IUnitOfWork<long> unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task CreateUserAsync(UserDomainModel user)
        {
            await CreateAsync(user);
        }

        public void CreateUser(UserDomainModel user)
        {
            Create(user);
        }

        public IEnumerable<UserDomainModel> GetUsersBy(Func<UserDomainModel, bool> func)
        {
            return GetBy(func);
        }
    }
}