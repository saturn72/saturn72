using Saturn72.Core.Configuration;

namespace Saturn72.Core.Services.Impl.User
{
    public class UserSettings:SettingsBase
    {
        public string HashedPasswordFormat { get; set; }
        public bool ValidateByEmail { get; set; }
        public bool ActivateUserAfterRegistration { get; set; }
    }
}