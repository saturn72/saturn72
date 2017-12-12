#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class DeletedEvent<TDomainModel> : CrudEventBase<TDomainModel> where TDomainModel : DomainModelBase
    {
        public DeletedEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}