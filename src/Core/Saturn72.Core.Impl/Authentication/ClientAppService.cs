#region

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Clients;
using Saturn72.Core.Services.Authentication;

#endregion

namespace Saturn72.Core.Services.Impl.Authentication
{
    public class ClientAppService : IClientAppService
    {
        #region consts

        private const string AllClientAppsCacheKey = "saturn72.all-client-apps";

        #endregion

        private readonly ICacheManager _cacheManager;
        private readonly IClientAppRepository _clientAppRepository;

        #region ctor

        public ClientAppService(IClientAppRepository clientAppRepository, ICacheManager cacheManager)
        {
            _clientAppRepository = clientAppRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        public ClientAppDomainModel GetClientAppByClientId(string clientId, string clientIpAddress)
        {
            return GetAllClientApps()
                .FirstOrDefault(
                    ca => ca.Active && (ca.ClientId == clientId) && Regex.IsMatch(clientIpAddress, ca.AllowedOrigin));
        }

        public IEnumerable<ClientAppDomainModel> GetAllClientApps()
        {
            return _cacheManager.Get(AllClientAppsCacheKey, () => _clientAppRepository.GetAll());
        }
    }
}