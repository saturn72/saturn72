#region

using FluentValidation;
using Saturn72.Common.WebApi.Models.Account;

#endregion

namespace Saturn72.Common.WebApi.Validation.Validators.Account
{
    public class UserRegistrationValidator : Saturn72ValidatorBase<UserRegistrationApiModel>
    {
        public UserRegistrationValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .When(x => ShouldUseUsername())
                .WithMessage("Username is mandatory")
                .Length(6, 124)
                .WithMessage("Username should be at least 6 characters, and max 124");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is mandatory");

            RuleFor(x => x.Password)
                .Equal(x => x.PasswordConfirm)
                .WithMessage("Passwords do not match")
                .Must(CheckPasswordComplexity)
                .WithMessage("PAssword does not match complexity");
        }

        private bool CheckPasswordComplexity(string password)
        {
            return true;
        }

        private bool ShouldUseUsername()
        {
            return true;
        }
    }
}