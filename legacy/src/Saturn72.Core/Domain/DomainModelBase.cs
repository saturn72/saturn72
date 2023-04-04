namespace Saturn72.Core.Domain
{
    public abstract class DomainModelBase : DomainModelBase<long>
    {

    }

    public abstract class DomainModelBase<TId>
    {
        public TId Id { get; set; }
    }
}