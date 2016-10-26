
using System;

namespace Saturn72.Core.Domain.Security
{
    public class RefreshTokenDomainModel:DomainModelBase<long>
    {
        public string Hash { get; set; }
        public string ClientId { get; set; }
        public string Subject { get; set; }
        public DateTime IssuedOnUtc { get; set; }
        public DateTime ExpiresOnUtc { get; set; }
        public string ProtectedTicket { get; set; }
    }
}
