#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Data;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.Data.Repositories
{
    public interface IUserRepository : IRepository<UserDomainModel, long>
    {
        Task CreateUserAsync(UserDomainModel user);

        void CreateUser(UserDomainModel user);

        IEnumerable<UserDomainModel> GetUsersBy(Func<UserDomainModel, bool> func);
    }
}