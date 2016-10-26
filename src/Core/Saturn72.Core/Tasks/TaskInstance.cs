#region

using System;
using Saturn72.Extensions;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Logging;

#endregion

namespace Saturn72.Core.Tasks
{
    public class TaskInstance
    {
        public TaskInstance(BackgroundTaskDomainModel taskMetaModel)
        {
            Type = taskMetaModel.TaskTypeUniqueIdentifier;
            Enabled = taskMetaModel.Active;
            StopOnError = taskMetaModel.StopOnError;
            Name = taskMetaModel.Name;
        }

        public string Name { get; private set; }

        public bool StopOnError { get; private set; }

        public bool Enabled { get; private set; }

        public string Type { get; private set; }

        public DateTime LastEndUtc { get; set; }

        /// <summary>
        ///     Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="dispose">A value indicating whether all instances should be disposed after task run</param>
        /// <param name="ensureRunOnOneWebFarmInstance">
        ///     A value indicating whether we should ensure this task is run on one farm
        ///     node at a time
        /// </param>
        public void Execute(bool throwException = false, bool dispose = true)
        {
            var taskInstance = CommonHelper.CreateInstance<ITask>(Type);

            if (taskInstance.IsNull() && throwException)
                throw new ArgumentException("Failed to create {0} instance", Type);

            AppEngine.Current.ExecuteInNewScope(() => ExecuteTaskInNewScopeAction(taskInstance, throwException));
        }

        private void ExecuteTaskInNewScopeAction(ITask taskInstance, bool throwException)
        {
            try
            {
                taskInstance.Execute();
            }
            catch (Exception exc)
            {
                Enabled = !StopOnError;

                LastEndUtc = DateTime.UtcNow;

                //log error
                var loggers = AppEngine.Current.ResolveAll<ILogger>();


                loggers.Error("Error while running the '{0}' schedule task. {1}".AsFormat(Name, exc.Message), exc);
                if (throwException)
                    throw;
            }
        }
    }
}