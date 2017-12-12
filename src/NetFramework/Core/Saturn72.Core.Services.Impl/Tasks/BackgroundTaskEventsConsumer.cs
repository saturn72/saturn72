#region

using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Services.Events;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Tasks
{
    public class BackgroundTaskEventsEventSubscriber : IEventSubscriber<
        CreatedEvent<BackgroundTaskExecutionDataDomainModel>>
    {
        private const string BackgroundTaskExecutionDataKey =
            "BackgroundTaskEventsConsumer.BackgroundTaskExecutionDataRepository.";

        private static ICacheManager _cacheManager;
        private readonly IBackgroundTaskExecutionDataRepository _backgroundTaskExeDataRepository;

        public BackgroundTaskEventsEventSubscriber(
            IBackgroundTaskExecutionDataRepository backgroundTaskExeDataRepository)
        {
            _backgroundTaskExeDataRepository = backgroundTaskExeDataRepository;
        }

        public void HandleEvent(CreatedEvent<BackgroundTaskExecutionDataDomainModel> eventMessage)
        {
            Guard.NotNull(new object[] {eventMessage, eventMessage.DomainModel});

            _backgroundTaskExeDataRepository.CreateTaskExecutionData(eventMessage.DomainModel);
        }
    }
}