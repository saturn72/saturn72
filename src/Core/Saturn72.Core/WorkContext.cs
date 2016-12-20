
namespace Saturn72.Core
{
    public class WorkContext<TUserId> : IWorkContext<TUserId>
    {
        public TUserId CurrentUserId { get;  set; }
    }
}