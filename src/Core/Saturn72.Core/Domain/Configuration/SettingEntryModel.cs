
namespace Saturn72.Core.Domain.Configuration
{
    public class SettingEntryModel:DomainModelBase
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
