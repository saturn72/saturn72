namespace Saturn72.Core
{
    public class WorkContext:IWorkContext
    {
        public long CurrentUserId { get; set; }
        public string CurrentUserIpAddress { get; set; }
        public string ClientId { get; set; }
    }
}