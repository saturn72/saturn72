using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
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
            var srv = new UserService(null, null, null, null);
            //on illegal userId
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(0));
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(-123));
            //On not exists userroles
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns((IEnumerable<UserRoleModel>)null);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.IsSet(It.IsAny<string>()))
                .Returns(false);

            //On not exists userroles
            srv = new UserService(userRepo.Object, null, cm.Object, null);
            typeof(NullReferenceException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(123));
            //On exists userroles
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns(new List<UserRoleModel>());

            srv = new UserService(userRepo.Object, null, cm.Object, null);
            typeof(NullReferenceException).ShouldBeThrownBy(() => srv.GetUserUserRolesByUserIdAsync(123));
        }

        [Test]
        public void UserService_GetUserUserRolesByUserId_ReturnsUserRolesCollection()
        {
            var retVal = new[]
            {
                new UserRoleModel
                {
                    Active = true,
                    Name = "UserRole1",
                    IsSystemRole = false
                },
                new UserRoleModel
                {
                    Active = true,
                    Name = "UserRole2",
                    IsSystemRole = true
                }
            };
            var expectedVal = new[]
            {
                new UserRoleModel
                {
                    Active = true,
                    Name = "UserRole1",
                    IsSystemRole = false
                },
                new UserRoleModel
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

            var srv = new UserService(userRepo.Object, null, cm.Object, null);

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

        [Test]
        public void UserService_GetUserPermissions_ReturnsNull()
        {
            var pRepo = new Mock<IUserRepository>();
            pRepo.Setup(r => r.GetUserPermissions(It.IsAny<long>()))
                .Returns(() => null);

            var srv = new UserService(null, null, null, null);
            srv.GetUserPermissionsAsync(111).Result.ShouldBeNull();
        }

        [Test]
        public void UserService_GetUserPermissions_ReturnsPermissions()
        {
            var expected = TestPermissionRecords.PermisisonCollection1;

            var pRepo = new Mock<IUserRepository>();
            pRepo.Setup(r => r.GetUserPermissions(It.IsAny<long>()))
                .Returns(() => expected);

            var srv = new UserService(null, null, null, null);
            var res = srv.GetUserPermissionsAsync(111).Result;

            res.Count().ShouldEqual(expected.Count());
            for (var i = 0; i < res.Count(); i++)
                res.ElementAt(i).PropertyValuesAreEquals(expected.ElementAt(i));
        }
    }
}