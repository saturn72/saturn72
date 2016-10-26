using System;

namespace Saturn72.Core.Domain
{
    public interface IDomainModelBase<out TId>
    {
        TId Id { get; }
    }

    public class DomainModelBase<TId> : IDomainModelBase<TId>
    {
        public TId Id { get; set; }
    }
}