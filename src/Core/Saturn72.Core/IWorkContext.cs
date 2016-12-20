namespace Saturn72.Core
{
    public interface IWorkContext<TUserId>
    {
        TUserId CurrentUserId { get; set; }
    }
}
