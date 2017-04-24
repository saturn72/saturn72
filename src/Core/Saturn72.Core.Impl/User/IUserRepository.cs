#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.Impl.User
{
    public interface IUserRepository
    {
        void Create(UserModel model);
        IEnumerable<UserModel> GetAll();
        void Update(UserModel user);
        UserModel GetById(long userId);
        IEnumerable<UserModel> GetUsersByUsername(string username);
        IEnumerable<UserModel> GetUsersByEmail(string email);
    }
}