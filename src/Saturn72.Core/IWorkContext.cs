namespace Saturn72.Core
{
    public interface IWorkContext
    {
        long CurrentUserId { get; }
        string CurrentUserIpAddress { get; }
        string ClientId { get; }
    }
}