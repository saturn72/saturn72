
using Saturn72.Core.Domain.Clients;

namespace Saturn72.Core.Services.Authentication
{
    public interface IClientAppService
    {
        /// <summary>
        /// gets client application 
        /// </summary>
        /// <param name="clientId">client application id</param>
        /// <returns>App</returns>
        ClientAppDomainModel GetClientByClientId(string clientId);
    }
}
