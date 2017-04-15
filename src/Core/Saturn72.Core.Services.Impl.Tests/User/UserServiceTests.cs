using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Impl.Security;
using Saturn72.Core.Services.Impl.User;
using Saturn72.Extensions;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Core.Services.Impl.Tests.User
{
    public class UserServiceTests
    {
        #region Get user by username

        [Test]
        public void UserService_GetUserByUsernameAsync_ReturnsUser()
        {
            var userRepo = new Mock<IUserRepository>();
            var result = new UserModel {Id = 3};
            userRepo.Setup(u => u.GetUsersByUsername(It.IsAny<string>())).Returns(new[]
            {
                result
            });

            var cm = new Mock<ICacheManager>();
            var srv = new UserService(userRepo.Object, null, cm.Object, null, null, null);
            srv.GetUserByUsernameAsync("ffff").Result.ShouldEqual(result);
        }
        
        [Test]
        public void UserService_GetUserByUsernameAsync_InvalidUsername()
        {
            var srv = new UserService(null, null, null, null, null, null);
            typeof(ArgumentException).ShouldBeThrownBy(() =>
            {
                try
                {

                    var res = srv.GetUserByUsernameAsync("   ").Result;
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
            typeof(ArgumentException).ShouldBeThrownBy(() =>
            {
                try
                {

                    var res = srv.GetUserByUsernameAsync(null).Result;
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });

            typeof(ArgumentException).ShouldBeThrownBy(() =>
            {
                try
                {

                    var res = srv.GetUserByUsernameAsync("").Result;
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
        }

        [Test]
        public void UserService_GetUserByUsernameAsync_ReturnsNull()
        {
            var userRepo = new Mock<IUserRepository>();
            IEnumerable<UserModel> uRepoResult = null;
            userRepo.Setup(u => u.GetUsersByUsername(It.IsAny<string>())).Returns(uRepoResult);
            var srv = new UserService(userRepo.Object, null, null, null, null, null);

            srv.GetUserByUsernameAsync("ffff").Result.ShouldBeNull();

            uRepoResult = new UserModel[] {};
            srv.GetUserByUsernameAsync("ffff").Result.ShouldBeNull();
        }

        [Test]
        public void UserService_GetUserByUsernameAsync_MultipleNonDeletedUsersWithSameUsername()
        {
            var username = "ffff";

            var userRepo = new Mock<IUserRepository>();
            var result = new UserModel {Id = 3, Username = username};
            userRepo.Setup(u => u.GetUsersByUsername(It.IsAny<string>())).Returns(new[]
            {
                result,
                new UserModel {Id = 1, Username = username, Deleted = true},
                new UserModel {Id = 2, Username = username},
                new UserModel {Id = 5, Username = username, Deleted = true}
            }.AsEnumerable());

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(new[] {LogLevel.Error});

            var cm = new Mock<ICacheManager>();
            var srv = new UserService(userRepo.Object, null, cm.Object, null, logger.Object, null);
            srv.GetUserByUsernameAsync(username).Result.ShouldEqual(result);
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Error), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<Guid>()), Times.Once);
        }

        #endregion

        #region Get user by email
        [Test]
        public void UserService_GetUserByEmailAsync_ReturnsNull()
        {
            var userRepo = new Mock<IUserRepository>();
            IEnumerable<UserModel> uRepoResult = null;
            userRepo.Setup(u => u.GetUsersByEmail(It.IsAny<string>())).Returns(uRepoResult);
            var srv = new UserService(userRepo.Object, null, null, null, null, null);

            srv.GetUserByEmailAsync("ttt@gmail.com").Result.ShouldBeNull();

            uRepoResult = new UserModel[] { };
            srv.GetUserByEmailAsync("ttt@gmail.com").Result.ShouldBeNull();
        }
        [Test]
        public void UserService_GetUserByEmailAsync_InvalidEmail()
        {
            var srv = new UserService(null, null, null, null, null, null);
            typeof(ArgumentException).ShouldBeThrownBy(() =>
            {
                try
                {

                    var res = srv.GetUserByEmailAsync("   ").Result;
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
            typeof(InvalidOperationException).ShouldBeThrownBy(() =>
            {
                try
                {

                    var res = srv.GetUserByEmailAsync("ttt").Result;
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            });
        }
        [Test]
        public void UserService_GetUserByEmailAsync_MultipleActiveUsersWithSameUsername()
        {
            var email = "ffff@ddd.com";

            var userRepo = new Mock<IUserRepository>();
            var result = new UserModel { Id = 3, Username = email };
            userRepo.Setup(u => u.GetUsersByEmail(It.IsAny<string>())).Returns(new[]
            {
                result,
                new UserModel {Id = 1, Username = email, Deleted = true},
                new UserModel {Id = 2, Username = email},
                new UserModel {Id = 5, Username = email, Deleted = true}
            }.AsEnumerable());

            var logger = new Mock<ILogger>();
            logger.Setup(l => l.SupportedLogLevels).Returns(new[] { LogLevel.Error });

            var cm = new Mock<ICacheManager>();
            var srv = new UserService(userRepo.Object, null, cm.Object, null, logger.Object, null);
            srv.GetUserByEmailAsync(email).Result.ShouldEqual(result);
            logger.Verify(
                l =>
                    l.InsertLog(It.Is<LogLevel>(ll => ll == LogLevel.Error), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public void UserService_GetUserByEmailAsync_ReturnsUser()
        {
            var userRepo = new Mock<IUserRepository>();
            var result = new UserModel { Id = 3 };
            userRepo.Setup(u => u.GetUsersByEmail(It.IsAny<string>())).Returns(new[]
            {
                result
            });

            var cm = new Mock<ICacheManager>();
            var srv = new UserService(userRepo.Object, null, cm.Object, null, null, null);
            srv.GetUserByEmailAsync("www@ffff.com").Result.ShouldEqual(result);
        }


        #endregion

        #region UserRoles

        [Test]
        public void UserService_GetUserUserRolesByUserId_Throws()
        {
            var srv = new UserService(null, null, null, null, null, null);
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
                .Returns(new string[] {});

            //Repository returns null
            var prRepo = new Mock<IPermissionRecordRepository>();
            prRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns((IEnumerable<UserRoleModel>) null);

            var srv1 = new UserService(null, null, cm.Object, null, null, prRepo.Object);
            var res1 = srv1.GetUserUserRolesByUserIdAsync(123).Result;
            res1.ShouldNotBeNull();
            res1.Count().ShouldEqual(0);


            //Repository returns empty collection
            prRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns(new List<UserRoleModel>());

            var srv2 = new UserService(null, null, cm.Object, null, null, prRepo.Object);
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
            var prRepo = new Mock<IPermissionRecordRepository>();
            prRepo.Setup(u => u.GetUserUserRoles(It.IsAny<long>()))
                .Returns(retVal);

            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Keys)
                .Returns(new string[] {});

            var srv = new UserService(null, null, cm.Object, null, null, prRepo.Object);
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
            var srv = new UserService(null, null, null, null, null, null);

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

            var prRepo = new Mock<IPermissionRecordRepository>();
            prRepo.Setup(r => r.GetUserPermissions(It.IsAny<long>()))
                .Returns(() => expected);

            var srv = new UserService(null, null, null, null, null, prRepo.Object);
            var res = srv.GetUserPermissionsAsync(111).Result;

            res.Count().ShouldEqual(expected.Count());
            for (var i = 0; i < res.Count(); i++)
                res.ElementAt(i).PropertyValuesAreEquals(expected.ElementAt(i));
        }

        #endregion
    }
}