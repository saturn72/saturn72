namespace Saturn72.Core
{
    public interface IWorkContext<TUserId>
    {
        TUserId CurrentUserId { get; set; }
        string CurrentUserIpAddress { get; set; }
        string ClientId { get; set; }
    }
}
