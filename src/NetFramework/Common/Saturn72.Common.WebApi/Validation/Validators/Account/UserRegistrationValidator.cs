#region

using FluentValidation;
using Saturn72.Common.WebApi.Models.Account;
using Saturn72.Core;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi.Validation.Validators.Account
{
    public class UserRegistrationValidator : Saturn72ValidatorBase<UserRegistrationApiModel>
    {
        public UserRegistrationValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required")
                .Length(6, 124)
                .WithMessage("Username should be at least 6 characters, and max 124");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required");
            RuleFor(x => x.Email)
                .Must(CommonHelper.IsValidEmail)
                .WithMessage("Illegal email address");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x.PasswordConfirm)
                .NotEmpty()
                .WithMessage("Password confirm is required");

            RuleFor(x => x.Password)
                .Equal(m => m.PasswordConfirm)
                .When(x => x.Password.NotEmptyOrNull())
                .When(x => x.PasswordConfirm.NotEmptyOrNull())
                .WithMessage("Passwords do not match");

            RuleFor(x => x.Password)
                .Must(CheckPasswordComplexity)
                .When(x => x.Password.NotEmptyOrNull())
                .When(x => x.PasswordConfirm.NotEmptyOrNull())
                .WithMessage("Password does not match complexity");
        }

        private bool CheckPasswordComplexity(string password)
        {
            return true;
        }
    }
}