#region

using Saturn72.Core.Data;
using Saturn72.Core.Domain.Tasks;

#endregion

namespace Saturn72.Core.Services.Data.Repositories
{
    public interface IBackgroundTaskRepository : IRepository<BackgroundTaskDomainModel, long>
    {
    }
}