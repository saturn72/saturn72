#region

using Saturn72.Core.Domain.Users;

#endregion

namespace Saturn72.Core.Services.User
{
    public class UserRegistrationRequest
    {
        #region ctor

        public UserRegistrationRequest(string usernameOrEmail, string password, PasswordFormat passwordFormat, string clientIp)
        {
            UsernameOrEmail = usernameOrEmail;
            Password = password;
            PasswordFormat = passwordFormat;
            ClientIp = clientIp;
        }


        #endregion


        public string UsernameOrEmail { get; private set; }
        public string Password { get; set; }
        public PasswordFormat PasswordFormat { get; private set; }
        public string PasswordSalt { get; set; }
        public string ClientIp { get; set; }

    }
}