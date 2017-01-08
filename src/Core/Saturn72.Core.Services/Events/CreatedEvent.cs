#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class CreatedEvent<TDomainModel> : CrudEventBase<TDomainModel> where TDomainModel : DomainModelBase
    {
        public CreatedEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}