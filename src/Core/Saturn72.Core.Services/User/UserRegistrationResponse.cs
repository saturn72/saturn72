#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Services.User
{
    public class UserRegistrationResponse
    {
        #region ctor

        public UserRegistrationResponse()
        {
            Errors = new List<string>();
        }

        #endregion

        public bool Success
        {
            get { return Errors.Count == 0; }
        }

        public IList<string> Errors { get; set; }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}