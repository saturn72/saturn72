
using Saturn72.Core.Domain.Clients;

namespace Saturn72.Core.Services.Authentication
{
    public interface IClientAppService
    {
        /// <summary>
        /// gets client application 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientIpAddress"></param>
        /// <returns>App</returns>
        ClientAppDomainModel GetClientAppByClientId(string clientId, string clientIpAddress);
    }
}
