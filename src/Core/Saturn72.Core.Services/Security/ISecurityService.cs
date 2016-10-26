namespace Saturn72.Core.Services.Security
{
    public interface ISecurityService
    {
        bool SetPricipal(string username, string password);
    }
}