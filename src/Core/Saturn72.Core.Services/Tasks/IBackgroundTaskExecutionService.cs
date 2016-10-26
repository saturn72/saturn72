using Saturn72.Core.Domain.Tasks;

namespace Saturn72.Core.Services.Tasks
{
    public interface IBackgroundTaskExecutionService
    {
        void CreateTaskExecutionData(BackgroundTaskExecutionDataDomainModel ted);
    }
}