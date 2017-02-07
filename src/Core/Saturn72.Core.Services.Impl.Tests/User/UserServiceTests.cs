using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.Impl.User;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class UserServiceTests
    {
        #region UserRoles

        [Test]
        public void UserService_GetUserUserRolesByUserId_Throws()
        {
            var srv = new UserService(null, null, null, null);
            //on illegal userId
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() =>
            {
                try
                {
                    srv.GetUserUserRolesByUserIdAsync(0).Wait();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() =>
            {
                try
                {
                    srv.GetUserUserRolesByUserIdAsync(-123).Wait();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
        }

        [Test]
        public void UserService_GetUserUserRolesByUserId_ReturnsEmptyCollection()
        {
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Keys)
                .Returns(new string[] { });

            //Repository returns null
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns((IEnumerable<UserRoleModel>)null);

            var srv1 = new UserService(userRepo.Object, null, cm.Object, null);
            var res1 = srv1.GetUserUserRolesByUserIdAsync(123).Result;
            res1.ShouldNotBeNull();
            res1.Count().ShouldEqual(0);


            //Repository returns empty collection
            userRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns(new List<UserRoleModel>());

            var srv2 = new UserService(userRepo.Object, null, cm.Object, null);
            var res2 = srv2.GetUserUserRolesByUserIdAsync(123).Result;
            res2.ShouldNotBeNull();
            res2.Count().ShouldEqual(0);
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
            cm.Setup(c => c.Keys)
                .Returns(new string[] { });

            var srv = new UserService(userRepo.Object, null, cm.Object, null);
            var userId = 123;
            var actural = srv.GetUserUserRolesByUserIdAsync(userId).Result;

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

            cm.Verify(
                c =>
                    c.Set(SystemSharedCacheKeys.UserRolesUserCacheKey.AsFormat(userId), It.IsAny<object>(),
                        It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Permissions

        [Test]
        public void UserService_GetUserPermissions_Throws()
        {
            var srv = new UserService(null, null, null, null);

            //on ilegal userId
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() =>
            {
                try
                {
                    srv.GetUserPermissionsAsync(0).Wait();
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            });
            typeof(ArgumentOutOfRangeException).ShouldBeThrownBy(() =>
            {
                try
                {
                    srv.GetUserPermissionsAsync(-123).Wait();
                }
                catch (Exception e)
                {
                    throw e.InnerException;
                }
            });
        }

        [Test]
        public void UserService_GetUserPermissions_ReturnsPermissions()
        {
            var expected = TestPermissionRecords.PermisisonCollection1;

            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetUserPermissions(It.IsAny<long>()))
                .Returns(() => expected);

            var srv = new UserService(userRepo.Object, null, null, null);
            var res = srv.GetUserPermissionsAsync(111).Result;

            res.Count().ShouldEqual(expected.Count());
            for (var i = 0; i < res.Count(); i++)
                res.ElementAt(i).PropertyValuesAreEquals(expected.ElementAt(i));
        }

        #endregion
    }
}