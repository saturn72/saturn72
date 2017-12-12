#region

using System.Collections.Generic;
using Saturn72.Core.Services.Validation;

#endregion

namespace Saturn72.Core.Services.User
{
    public class UserRegistrationResponse
    {
        #region ctor

        public UserRegistrationResponse()
        {
            Errors = new List<SystemErrorCode>();
        }

        #endregion

        public bool Success => Errors.Count == 0;

        public IList<SystemErrorCode> Errors { get; set; }

        public void AddError(SystemErrorCode errCode)
        {
            Errors.Add(errCode);
        }
    }
}