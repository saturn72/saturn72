using Saturn72.Core.Domain.Tasks;

namespace Saturn72.Core.Services.Data.Repositories
{
    public interface IBackgroundTaskExecutionDataRepository
    {
        void CreateTaskExecutionData(BackgroundTaskExecutionDataDomainModel ted);
    }
}