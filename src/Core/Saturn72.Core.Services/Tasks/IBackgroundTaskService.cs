#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Tasks;

#endregion

namespace Saturn72.Core.Services.Tasks
{
    public interface IBackgroundTaskService
    {
        Task UpdateScehduleTaskAsync(BackgroundTaskDomainModel task);
        Task<IEnumerable<BackgroundTaskDomainModel>> GetAllTasksAsync();
        IEnumerable<BackgroundTaskDomainModel> GetAllTasks();
        Task<BackgroundTaskDomainModel> CreateTaskAsync(BackgroundTaskDomainModel task);
        Task<IEnumerable<IBackgroundTaskType>> GetAllTaskTypesAsync();
        IEnumerable<IBackgroundTaskType> GetAllTaskTypes();
        Task<IEnumerable<string>> GetAllFullTrustBackgroundTaskTypeNames();
        Task<BackgroundTaskDomainModel> GetTaskByIdAsync(long id);
        Task DeleteBackgroundTask(long id);
        Task RunNow(long taskId);

    }
}