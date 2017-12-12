#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Tasks;

#endregion

namespace Saturn72.Module.TaskManager
{
    public class TaskManagerModule : IModule
    {
        private ITaskManager _taskManager;
        public void Load()
        {
            _taskManager = AppEngine.Current.Resolve<ITaskManager>();

            Console.Out.WriteLine("Initizlize task manager...");
            _taskManager.Initialize();
        }

        public void Start()
        {
            Console.Out.WriteLine("Starts task manager...");
            _taskManager.Start();

        }

        public void Stop()
        {
            Console.Out.WriteLine("Stops task manager...");
            _taskManager.Stop();
        }
    }
}