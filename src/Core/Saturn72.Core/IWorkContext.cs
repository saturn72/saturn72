namespace Saturn72.Core
{
    public interface IWorkContext
    {
        long CurrentUserId { get; set; }
        string CurrentUserIpAddress { get; set; }
        string ClientId { get; set; }
    }
}
