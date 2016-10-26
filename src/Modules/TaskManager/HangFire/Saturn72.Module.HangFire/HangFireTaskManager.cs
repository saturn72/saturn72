#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hangfire;
using Saturn72.Core;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Tasks;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.HangFire
{
    public class HangFireTaskManager : ResolverBase, ITaskManager
    {
        private const string AllTaskTypesKey = "TaskTypes.";
        private static ICacheManager _cacheManager;
        private readonly IBackgroundTaskRepository _backgroundTaskRepository;

        public HangFireTaskManager(IBackgroundTaskRepository backgroundTaskRepository)
        {
            _backgroundTaskRepository = backgroundTaskRepository;
        }

        public void Initialize()
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            {
                Attempts = 0
                //OnAttemptsExceeded=AttemptsExceededAction.Fail
            });

            //hangfire saves the executio data in database.
        }

        public void Start()
        {
            //hangfire starts automatically
        }

        public void Stop()
        {
            //TODO:
        }

        public object UpdateTask(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            if (backgroundTaskDomainModel.Active)
                return Schedule(backgroundTaskDomainModel);

            Remove(backgroundTaskDomainModel);
            return backgroundTaskDomainModel.Id;
        }

        public void Remove(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            var taskId = GetBackgroundTaskId(backgroundTaskDomainModel);
            if (IsRecurring(backgroundTaskDomainModel))
            {
                RecurringJob.RemoveIfExists(taskId);
            }
            else
            {
                BackgroundJob.Delete(taskId);
            }
        }

        public object Schedule(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            Guard.NotNull(backgroundTaskDomainModel);

            var taskId = GetBackgroundTaskId(backgroundTaskDomainModel);
            var tType = GetBackgroundTaskBackgroundTaskType(backgroundTaskDomainModel);
            var methodCall = CreateMethodCallExpression(backgroundTaskDomainModel, tType);

            //recurring task
            if (IsRecurring(backgroundTaskDomainModel))
            {
                RecurringJob.AddOrUpdate(taskId, methodCall, backgroundTaskDomainModel.Cron);
                return taskId;
            }
            //if run once task
            return backgroundTaskDomainModel.DelayTimeSpan == default(TimeSpan)
                ? BackgroundJob.Enqueue(methodCall)
                : BackgroundJob.Schedule(methodCall, backgroundTaskDomainModel.DelayTimeSpan);
        }

        public void RunNow(object taskId)
        {
            //Get ScheduledTaskMetaModel type and trigger accordingly
            var manager = new RecurringJobManager();
            manager.Trigger(taskId.ToString());
        }

        private static bool IsRecurring(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            return backgroundTaskDomainModel.Cron == default(string);
        }

        private static string GetBackgroundTaskId(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            return backgroundTaskDomainModel.Id.ToString();
        }

        private static IBackgroundTaskType GetBackgroundTaskBackgroundTaskType(
            BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            var tType =
                CachedTaskTypes()
                    .FirstOrDefault(
                        tt =>
                            tt.UniqueIdentifier.EqualsTo(backgroundTaskDomainModel.TaskTypeUniqueIdentifier));
            Guard.NotNull(tType);
            return tType;
        }

        private Expression<Action> CreateMethodCallExpression(BackgroundTaskDomainModel backgroundTaskDomainModel,
            IBackgroundTaskType tType)
        {
            var prms = backgroundTaskDomainModel.Parameters
                .ToDictionary(k => k.Key, k => (object) k.Value, StringComparer.InvariantCultureIgnoreCase);

            prms["Id"] = backgroundTaskDomainModel.Id;

            return () => JobRunner.Run(CommonHelper.GetCompatibleTypeName(tType.ExecutorType), prms);
        }


        protected static IEnumerable<IBackgroundTaskType> CachedTaskTypes()
        {
            return (_cacheManager ?? (_cacheManager = AppEngine.Current.Resolve<ICacheManager>()))
                .Get(AllTaskTypesKey, 1440, () =>
                {
                    var result = new List<IBackgroundTaskType>();
                    AppEngine.Current.Resolve<ITypeFinder>()
                        .FindClassesOfTypeAndRunMethod<IBackgroundTaskType>(result.Add);
                    return result;
                });
        }
    }
}