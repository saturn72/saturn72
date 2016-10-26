#region

using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Tasks;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Tasks
{
    public class BackgroundTaskExecutionService : IBackgroundTaskExecutionService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IBackgroundTaskExecutionDataRepository _taskExecutionDataRepository;


        public BackgroundTaskExecutionService(IBackgroundTaskExecutionDataRepository taskExecutionDataRepository,
            IEventPublisher eventPublisher)
        {
            _taskExecutionDataRepository = taskExecutionDataRepository;
            _eventPublisher = eventPublisher;
        }

        public void CreateTaskExecutionData(BackgroundTaskExecutionDataDomainModel ted)
        {
            Guard.NotNull(ted);

            _taskExecutionDataRepository.CreateTaskExecutionData(ted);
            _eventPublisher.DomainModelCreated<BackgroundTaskExecutionDataDomainModel, long>(ted);
        }
    }
}