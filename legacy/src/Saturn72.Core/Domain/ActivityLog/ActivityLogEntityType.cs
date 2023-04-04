#region Usings

using System;
using Saturn72.Core.Domain.Identity;

#endregion

namespace Saturn72.Core.Domain.ActivityLog
{
    public class ActivityLogEntityType
    {
        #region Members

        public static readonly ActivityLogEntityType User =
            new ActivityLogEntityType(10, typeof(UserModel));

        #endregion

        #region Properties

        public int Code { get; }
        public Type EntityType { get; }

        #endregion

        #region Ctor

        protected ActivityLogEntityType(int code, Type entityType)
        {
            Code = code;
            EntityType = entityType;
        }

        #endregion
    }
}