#region

using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.User
{
    public class UserRegistrationRequest
    {
        #region ctor

        public UserRegistrationRequest(string username, string email, string password, PasswordFormat passwordFormat, string clientIp)
        {
            Username = username;
            Email = email;
            Password = password;
            PasswordFormat = passwordFormat;
            ClientIp = clientIp;
        }


        #endregion
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string Password { get; set; }
        public PasswordFormat PasswordFormat { get; private set; }
        public string PasswordSalt { get; set; }
        public string ClientIp { get; private set; }

    }
}