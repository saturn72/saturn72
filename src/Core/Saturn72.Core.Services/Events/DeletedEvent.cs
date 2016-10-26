#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class DeletedEvent<TDomainModel, TId> : CrudEventBase<TDomainModel, TId> where TDomainModel : DomainModelBase<TId>
    {
        public DeletedEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}