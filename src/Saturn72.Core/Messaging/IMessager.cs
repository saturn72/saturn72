namespace Saturn72.Core.Messaging
{
    public interface IMessager
    {
        Task SendAsync(Message message);
    }
}