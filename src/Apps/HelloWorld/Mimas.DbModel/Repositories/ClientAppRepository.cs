using Saturn72.Common.Data.Repositories;
using Saturn72.Core.Domain.Clients;
using Saturn72.Core.Services.Data.Repositories;

namespace Mimas.DbModel.Repositories
{
    public class ClientAppRepository : RepositoryBase<ClientAppDomainModel, long, ClientApp>,
        IClientAppRepository
    {
        public ClientAppRepository(IUnitOfWork<long> unitOfWork) : base(unitOfWork)
        {
        }
    }
}