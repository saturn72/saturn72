using Saturn72.Core.Configuration;

namespace Saturn72.Core.Services.Impl.User
{
    public class UserSettings:SettingsBase
    {
        public virtual string HashedPasswordFormat { get; set; }
        public virtual bool ValidateByEmail { get; set; }
        public virtual bool ActivateUserAfterRegistration { get; set; }
    }
}