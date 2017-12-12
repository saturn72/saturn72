#region

using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Extensions;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Tasks
{
    public class MemoryTaskManager : ITaskManager
    {
        private readonly List<TaskThread> _taskThreads = new List<TaskThread>();

        public void Initialize()
        {
            _taskThreads.Clear();

            var typeFinder = AppEngine.Current.Resolve<ITypeFinder>();
            var tasksInstances = new List<BackgroundTaskDomainModel>();
            typeFinder.FindClassesOfTypeAndRunMethod<BackgroundTaskDomainModel>(t =>
            {
                if (t.Active)
                    tasksInstances.Add(t);
            });

            //group by threads with the same Seconds
            foreach (var backgroundTaskGroup in tasksInstances.GroupBy(x => x.Seconds))
            {
                //create a thread
                var taskThread = new TaskThread
                {
                    Seconds = backgroundTaskGroup.Key
                };

                foreach (var scheduleTask in backgroundTaskGroup)
                {
                    var task = new TaskInstance(scheduleTask);
                    taskThread.AddTask(task);
                }
                _taskThreads.Add(taskThread);
            }
        }

        public void Start()
        {
            _taskThreads.ForEachItem(tt => tt.InitTimer());
        }

        public object Schedule(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            var taskInstance = new TaskInstance(backgroundTaskDomainModel);
            var taskThread = _taskThreads.FirstOrDefault(x => x.Seconds == backgroundTaskDomainModel.Seconds);

            if (taskThread.IsNull())
            {
                taskThread = new TaskThread();
                _taskThreads.Add(taskThread);
            }

            taskThread.AddTask(taskInstance);

            return backgroundTaskDomainModel;
        }

        public void RunNow(object taskId)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            _taskThreads.ForEachItem(tt => tt.Dispose());
        }

        public object UpdateTask(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            throw new NotImplementedException();
        }

        public void Remove(BackgroundTaskDomainModel backgroundTaskDomainModel)
        {
            throw new NotImplementedException();
        }
    }
}