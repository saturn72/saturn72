
using FluentValidation.Attributes;
using Saturn72.Common.WebApi.Validation.Validators.Account;

namespace Saturn72.Common.WebApi.Models.Account
{
    [Validator(typeof(UserRegistrationValidator))]
    public class UserRegistrationApiModel:ApiModelBase
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}
