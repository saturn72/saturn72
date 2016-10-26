#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public abstract class CrudEventBase<TDomainModel, TId> : EventBase where TDomainModel : DomainModelBase<TId>
    {
        protected CrudEventBase(TDomainModel domainModel)
        {
            DomainModel = domainModel;
        }

        public TDomainModel DomainModel { get; set; }
    }
}