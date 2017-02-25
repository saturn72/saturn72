using System.Collections.Generic;
using Saturn72.Core.Services.User;
using Saturn72.Core.Services.Validation;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.User
{
    public class UsernameAndEmailRegistrationRequestValidator : IUserRegistrationRequestValidator
    {
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;

        public UsernameAndEmailRegistrationRequestValidator(IUserService userService, UserSettings userSettings)
        {
            _userService = userService;
            _userSettings = userSettings;
        }

        public IEnumerable<SystemErrorCode> ValidateRequest(UserRegistrationRequest request)
        {
            var response = new List<SystemErrorCode>();
            //Check username
            var usernameOrEmailNotEmpty = request.Username.HasValue();
            if (!usernameOrEmailNotEmpty)
                response.Add("Please specify user email or username");

            if (usernameOrEmailNotEmpty && !_userSettings.ValidateByEmail &&
                _userService.GetUserByUsername(request.Username).NotNull())
                response.Add("Username already exists");

            if (usernameOrEmailNotEmpty && _userSettings.ValidateByEmail &&
                _userService.GetUserByEmail(request.Username).NotNull())
                response.Add("Email already exists");

            return response;
        }
    }
}