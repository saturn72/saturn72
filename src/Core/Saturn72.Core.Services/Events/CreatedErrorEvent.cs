using Saturn72.Core.Domain;

namespace Saturn72.Core.Services.Events
{
    public class CreatedErrorEvent<TDomainModel> : CrudEventBase<TDomainModel> where TDomainModel : DomainModelBase
    {
        public CreatedErrorEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}