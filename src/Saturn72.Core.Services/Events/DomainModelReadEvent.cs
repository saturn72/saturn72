using Saturn72.Core.Domain;

namespace Saturn72.Core.Services.Events
{
    public class DomainModelReadEvent<TDomainModel> : EventBase
    where TDomainModel : DomainModelBase
    {
        public TDomainModel Model { get; set; }
        public bool WasFound { get; set; }
    }
}