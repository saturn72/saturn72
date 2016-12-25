using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Data.Repositories;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Impl.User;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class UserServiceTests
    {
        [Test]
        public void UserService_GetUserUserRolesByUserId_Throws()
        {
            var srv = new UserService(null, null, null, null,null);
            //on illegal userId
            typeof(InvalidOperationException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(0));
            typeof(InvalidOperationException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(-123));
            //On not exists userroles
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns((IEnumerable<UserRoleDomainModel>)null);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.IsSet(It.IsAny<string>()))
                .Returns(false);

            srv = new UserService(userRepo.Object, null, cm.Object, null, null);
            typeof(NullReferenceException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(123));
            //On not exists userroles
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns(new List<UserRoleDomainModel>());

            srv = new UserService(userRepo.Object, null, cm.Object, null, null);
            typeof(InvalidOperationException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(123));
        }

        [Test]
        public void UserService_GetUserUserRolesByUserId_ReturnsUserRolesCollection()
        {
            var retVal = new[]
            {
                new UserRoleDomainModel
                {
                    Active = true,
                    Name = "UserRole1",
                    IsSystemRole = false
                },
                new UserRoleDomainModel
                {
                    Active = true,
                    Name = "UserRole2",
                    IsSystemRole = true
                }
            };
            var expectedVal = new[]
            {
                new UserRoleDomainModel
                {
                    Active = true,
                    Name = "UserRole1",
                    IsSystemRole = false
                },
                new UserRoleDomainModel
                {
                    Active = true,
                    Name = "UserRole2",
                    IsSystemRole = true
                }
            };
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns(retVal);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.IsSet(It.IsAny<string>()))
                .Returns(false);

            var srv = new UserService(userRepo.Object, null, cm.Object, null, null);

            var actural = srv.GetUserUserRolesByUserIdAsync(123).Result;

            for (var i = 0; i < expectedVal.Length; i++)
            {
                var act = actural.ElementAt(i);
                var exp = expectedVal[i];

                act.Active.ShouldEqual(exp.Active);
                act.IsSystemRole.ShouldEqual(exp.IsSystemRole);
                act.Name.ShouldEqual(exp.Name);
                act.PermissionRecords.ShouldEqual(exp.PermissionRecords);
                act.SystemName.ShouldEqual(exp.SystemName);
            }

            var cacheKey = "Saturn72_User{0}_UserRoles";

            cm.Verify(c => c.Set(cacheKey, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        //Validate caching
    }
}