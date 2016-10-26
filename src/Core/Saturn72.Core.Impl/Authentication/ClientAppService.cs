#region

using System.Linq;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Clients;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Authentication;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;

#endregion

namespace Saturn72.Core.Services.Impl.Authentication
{
    public class ClientAppService : DomainModelCrudServiceBase<ClientAppDomainModel, long>, IClientAppService
    {
        #region ctor

        public ClientAppService(IClientAppRepository clientAppRepository, IEventPublisher eventPublisher,
            ICacheManager cacheManager, ITypeFinder typeFinder)
            : base(clientAppRepository, eventPublisher, cacheManager, typeFinder)
        {
        }

        #endregion

        public ClientAppDomainModel GetClientByClientId(string clientId)
        {
            return FilterTable(ca => ca.Active && (ca.ClientId == clientId)).FirstOrDefault();
        }
    }
}