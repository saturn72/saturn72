using System.Collections.Generic;
using Saturn72.Core.Services.User;

namespace Saturn72.Core.Services.Impl.User
{
    public abstract class UserRegistrationRequestValidatorBase
    {
        public abstract IEnumerable<string> ValidateRequest(UserRegistrationRequest request);
    }
}