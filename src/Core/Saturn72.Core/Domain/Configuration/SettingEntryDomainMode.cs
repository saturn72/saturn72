
namespace Saturn72.Core.Domain.Configuration
{
    public class SettingEntryDomainModel:DomainModelBase<long>
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
