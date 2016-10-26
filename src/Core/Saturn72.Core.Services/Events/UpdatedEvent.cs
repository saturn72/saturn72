#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class UpdatedEvent<TDomainModel, TId> : CrudEventBase<TDomainModel, TId> where TDomainModel : DomainModelBase<TId>
    {
        public UpdatedEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}