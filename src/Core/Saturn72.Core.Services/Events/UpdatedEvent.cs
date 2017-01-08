#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class UpdatedEvent<TDomainModel> : CrudEventBase<TDomainModel> where TDomainModel : DomainModelBase
    {
        public UpdatedEvent(TDomainModel domainModel) : base(domainModel)
        {
        }
    }
}