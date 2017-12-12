#region

using Saturn72.Core.Domain.Tasks;

#endregion

namespace Saturn72.Core.Tasks
{
    public interface ITaskManager
    {
        /// <summary>
        ///     Initializes the scheduledTask manager intance.
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Starts the taks manager execution.
        /// </summary>
        void Start();

        /// <summary>
        ///     Adds background scheduledTask
        /// </summary>
        /// <param name="backgroundTaskDomainModel">
        ///     background scheduledTask wrapper <see cref="BackgroundTaskDomainModel" />
        /// </param>
        object Schedule(BackgroundTaskDomainModel backgroundTaskDomainModel);

        /// <summary>
        ///     Executes scheduledTask
        /// </summary>
        /// <param name="taskId">scheduledTask code</param>
        void RunNow(object taskId);

        /// <summary>
        ///     Stops the task manager
        /// </summary>
        void Stop();

        /// <summary>
        /// Updates background task
        /// </summary>
        /// <param name="backgroundTaskDomainModel"></param>
        object UpdateTask(BackgroundTaskDomainModel backgroundTaskDomainModel);

        /// <summary>
        /// Removes task from task pool
        /// </summary>
        /// <param name="backgroundTaskDomainModel"></param>
        void Remove(BackgroundTaskDomainModel backgroundTaskDomainModel);
    }
}