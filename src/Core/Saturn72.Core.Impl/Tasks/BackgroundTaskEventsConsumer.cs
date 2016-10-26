#region

using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Tasks;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Tasks
{
    public class BackgroundTaskEventsConsumer : IConsumer<CreatedEvent<BackgroundTaskExecutionDataDomainModel, long>>
    {
        private const string BackgroundTaskExecutionDataKey =
            "BackgroundTaskEventsConsumer.BackgroundTaskExecutionDataRepository.";

        private static ICacheManager _cacheManager;

        private static ICacheManager CacheManager
        {
            get { return _cacheManager ?? (_cacheManager = AppEngine.Current.Resolve<ICacheManager>()); }
        }

        private IBackgroundTaskExecutionDataRepository BackgroundTaskExeDataRepository
        {
            get
            {
                return CacheManager.Get(BackgroundTaskExecutionDataKey,
                    () => AppEngine.Current.Resolve<IBackgroundTaskExecutionDataRepository>());
            }
        }

        public void HandleEvent(CreatedEvent<BackgroundTaskExecutionDataDomainModel, long> eventMessage)
        {
            Guard.NotNull(new object[] {eventMessage, eventMessage.DomainModel});

            BackgroundTaskExeDataRepository.CreateTaskExecutionData(eventMessage.DomainModel);
        }
    }
}