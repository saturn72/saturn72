#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class CreatedEvent<TDomainModel, TId> : CrudEventBase<TDomainModel, TId> where TDomainModel : DomainModelBase<TId>
    {
        public CreatedEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}