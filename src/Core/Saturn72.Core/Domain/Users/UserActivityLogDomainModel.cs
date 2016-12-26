using System;

namespace Saturn72.Core.Domain.Users
{
    public class UserActivityLogDomainModel : DomainModelBase<long>
    {
        public long UserId { get; set; }
        public string ActivityTypeCode { get; set; }
        public DateTime ActivityDateUtc { get; set; }
        public string UserIpAddress { get; set; }
        public string ClientAppId { get; set; }
    }
}