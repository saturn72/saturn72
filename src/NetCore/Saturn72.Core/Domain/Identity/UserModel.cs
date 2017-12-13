using System;
using Saturn72.Core.Audit;

namespace Saturn72.Core.Domain.Identity
{
    public class UserModel : DomainModelBase, IFullAudit
    {
        public string Username { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public long CreatedByUserId { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public long? DeletedByUserId { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public long? UpdatedByUserId { get; set; }
        public bool Active { get; set; }
    }
}