#region

using System.Collections.Generic;
using Saturn72.Core.Domain.Security;

#endregion

namespace Saturn72.Core.Domain.Users
{
    public class UserRoleModel : DomainModelBase
    {
        private ICollection<PermissionRecordModel> _permissionRecords;

        /// <summary>
        ///     Gets or sets the customer role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the customer role is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the customer role is system
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        ///     Gets or sets the customer role system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        ///     Gets or sets the permission records
        /// </summary>
        public virtual ICollection<PermissionRecordModel> PermissionRecords
        {
            get { return _permissionRecords ?? (_permissionRecords = new List<PermissionRecordModel>()); }
            protected set { _permissionRecords = value; }
        }
    }
}