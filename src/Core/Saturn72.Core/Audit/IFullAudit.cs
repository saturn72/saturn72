
namespace Saturn72.Core.Audit
{
    public interface IFullAudit<TUserId>:IUpdatedAudit<TUserId>, IDeletedAudit<TUserId>
    {
    }
}
