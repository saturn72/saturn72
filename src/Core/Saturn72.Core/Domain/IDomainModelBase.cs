namespace Saturn72.Core.Domain
{
    public interface IDomainModelBase<out TId>
    {
        TId Id { get; }
    }

    public class DomainModelBase : IDomainModelBase<long>
    {
        public long Id { get; set; }
    }
}