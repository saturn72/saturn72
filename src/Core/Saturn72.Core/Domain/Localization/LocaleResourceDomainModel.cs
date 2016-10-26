
namespace Saturn72.Core.Domain.Localization
{
    public class LocaleResourceDomainModel:DomainModelBase<long>
    {

        public string Key { get; set; }

        public string Value { get; set; }

        public int LanguageId { get; set; }
    }
}
