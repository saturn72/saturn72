#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Audit;

#endregion

namespace Saturn72.Core.Domain.Users
{
    public class UserModel : DomainModelBase, ICrudAudit, IBrowseDataAudit
    {
        #region Fields

        private ICollection<UserRoleModel> _userRoles;

        #endregion

        public UserModel()
        {
            UserGuid = Guid.NewGuid();
            PasswordFormat = PasswordFormat.Clear;
        }

        public Guid UserGuid { get; set; }

        /// <summary>
        ///     Gets or sets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        public PasswordFormat PasswordFormat { get; set; }

        public string PasswordSalt { get; set; }

        /// <summary>
        ///     Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        //Audit
        public string LastClientAppId { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime LastBrowsedOnUtc { get; set; }
        public virtual ICollection<UserRoleModel> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRoleModel>()); }
        }
        public DateTime CreatedOnUtc { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public long? UpdatedByUserId { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public long? DeletedByUserId { get; set; }
    }
}