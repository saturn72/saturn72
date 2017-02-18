using System;
using Saturn72.Core.Domain;

namespace Saturn72.Core.Audit
{
    public class SuspensionReason : DomainModelBase
    {
        public Guid Code { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
    }
}