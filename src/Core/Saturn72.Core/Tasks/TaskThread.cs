#region

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Saturn72.Core.Tasks
{
    public class TaskThread : IDisposable
    {
        private readonly IDictionary<string, TaskInstance> _tasks;
        private bool _disposed;
        private Timer _timer;

        internal TaskThread()
        {
            _tasks = new Dictionary<string, TaskInstance>();
            Seconds = 10*60;
        }


        public long Seconds { get; set; }

        /// <summary>
        ///     Gets the interval at which to run the tasks
        /// </summary>
        public long Interval
        {
            get { return Seconds*1000; }
        }

        public bool IsRunning { get; set; }

        public DateTime StartedUtc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the thread whould be run only once (per appliction start)
        /// </summary>
        public bool RunOnlyOnce { get; set; }

        /// <summary>
        ///     Disposes the instance
        /// </summary>
        public void Dispose()
        {
            if ((_timer != null) && !_disposed)
            {
                lock (this)
                {
                    _timer.Dispose();
                    _timer = null;
                    _disposed = true;
                }
            }
        }

        public void AddTask(TaskInstance task)
        {
            if (!_tasks.ContainsKey(task.Name))
            {
                _tasks.Add(task.Name, task);
            }
        }

        public void InitTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer(TimerHandler, null, Interval, Interval);
            }
        }

        private void TimerHandler(object state)
        {
            _timer.Change(-1, -1);
            Run();
            if (RunOnlyOnce)
            {
                Dispose();
            }
            else
            {
                _timer.Change(Interval, Interval);
            }
        }

        private void Run()
        {
            if (Seconds <= 0)
                return;

            StartedUtc = DateTime.UtcNow;
            IsRunning = true;
            foreach (var task in _tasks.Values)
            {
                task.Execute();
            }
            IsRunning = false;
        }
    }
}