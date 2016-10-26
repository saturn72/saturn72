#region

using System;
using System.Linq;

#endregion

namespace Saturn72.Core.Domain.Users
{
    public static class UserExtensions
    {
        /// <summary>
        ///     Gets a value indicating whether user is registered
        /// </summary>
        /// <param name="user">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this UserDomainModel user, bool onlyActiveCustomerRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Registered, onlyActiveCustomerRoles);
        }

        /// <summary>
        ///     Gets a value indicating whether user is in a certain user role
        /// </summary>
        /// <param name="user">Customer</param>
        /// <param name="userRoleSystemName">Customer role system name</param>
        /// <param name="onlyActiveUserRoles">A value indicating whether we should look only in active user roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this UserDomainModel user, string userRoleSystemName,
            bool onlyActiveUserRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrEmpty(userRoleSystemName))
                throw new ArgumentNullException("userRoleSystemName");

            var result = user.UserRoles
                .FirstOrDefault(cr => (!onlyActiveUserRoles || cr.Active) && (cr.SystemName == userRoleSystemName)) !=
                         null;
            return result;
        }
    }
}