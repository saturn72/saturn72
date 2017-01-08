#region

using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public abstract class CrudEventBase<TDomainModel> : EventBase where TDomainModel : DomainModelBase
    {
        protected CrudEventBase(TDomainModel domainModel)
        {
            DomainModel = domainModel;
        }

        public TDomainModel DomainModel { get; set; }
    }
}