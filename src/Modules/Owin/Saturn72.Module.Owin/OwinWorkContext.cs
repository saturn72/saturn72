using Saturn72.Core.Services;

namespace Saturn72.Module.Owin
{
    public class OwinWorkContext<TUserId> : IWorkContext<TUserId>
    {
        public TUserId CurrentUserId { get; internal set; }
    }
}