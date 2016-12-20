namespace Saturn72.Core.Services
{
    public interface IWorkContext<out TUserId>
    {
        TUserId CurrentUserId { get; }
    }
}
