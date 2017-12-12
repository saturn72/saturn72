using System.Collections.Generic;
using Saturn72.Core.Services.User;
using Saturn72.Core.Services.Validation;

namespace Saturn72.Core.Services.Impl.User
{
    public interface IUserRegistrationRequestValidator
    {
        IEnumerable<SystemErrorCode> ValidateRequest(UserRegistrationRequest request);
    }
}