namespace Saturn72.Core
{
    public interface IWorkContext<out TUserId>
    {
        TUserId CurrentUserId { get; }
    }
}
