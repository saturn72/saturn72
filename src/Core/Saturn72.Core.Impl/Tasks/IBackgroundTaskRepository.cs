#region

using Saturn72.Core.Data;
using Saturn72.Core.Domain.Tasks;

#endregion

namespace Saturn72.Core.Services.Impl.Tasks
{
    public interface IBackgroundTaskRepository : IRepository<BackgroundTaskDomainModel>
    {
    }
}