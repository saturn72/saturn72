using Saturn72.Core.Domain.Tasks;

namespace Saturn72.Core.Services.Impl.Tasks
{
    public interface IBackgroundTaskExecutionDataRepository
    {
        void CreateTaskExecutionData(BackgroundTaskExecutionDataDomainModel ted);
    }
}