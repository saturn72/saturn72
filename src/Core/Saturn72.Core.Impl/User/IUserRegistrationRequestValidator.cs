using System.Collections.Generic;
using Saturn72.Core.Services.User;

namespace Saturn72.Core.Services.Impl.User
{
    public interface IUserRegistrationRequestValidator
    {
        IEnumerable<string> ValidateRequest(UserRegistrationRequest request);
    }
}