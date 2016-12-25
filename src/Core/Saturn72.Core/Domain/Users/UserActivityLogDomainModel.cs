using System;

namespace Saturn72.Core.Domain.Users
{
    public class UserActivityLogDomainModel : DomainModelBase<long>
    {
        public Guid UserGuid { get; set; }
        public string ActvityTypeCode { get; set; }
        public DateTime ActivityDateUtc { get; set; }
        public string UserIpAddress { get; set; }
        public string ClientApp { get; set; }
    }
}