using System;

namespace Saturn72.Core.Domain.Users
{
    public class UserActivityLogModel : DomainModelBase
    {
        public long UserId { get; set; }
        public string ActivityTypeCode { get; set; }
        public DateTime ActivityDateUtc { get; set; }
        public string UserIpAddress { get; set; }
        public string ClientAppId { get; set; }
        public string ActivityTypeSystemName { get; set; }
    }
}