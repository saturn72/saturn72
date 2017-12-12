#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Saturn72.Core.Configuration;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.App.Events;
using Saturn72.Core.Services.Events;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.App
{
    public abstract class Saturn72AppBase : IApp
    {
        public virtual void Start()
        {
            Trace.WriteLine("Start {0} application".AsFormat(_appId));

            _data = ConfigManager.AppDomainLoadData;
            Trace.WriteLine("Read configuration file data and load external assemblies...");
            AppDomainLoader.Load(_data);

            Trace.WriteLine("Start application engine...");
            AppEngine.Initialize(true);

            _eventPublisher = AppEngine.Current.Resolve<IEventPublisher>();
            _eventPublisher.Publish(new OnApplicationInitializeStartEvent(this));

            Trace.WriteLine("Start all modules...");
            StartAllModules(_data);

            _eventPublisher.Publish(new OnApplicationInitializeFinishEvent(this));

            Trace.WriteLine("Press CTRL+C to quit application...");

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();

            Trace.WriteLine("Stop all modules...");
            StopAllModules(_data);
            AppEngine.Current.Dispose();
            DisplayExitCounter();
        }

        public virtual void Stop()
        {
            _eventPublisher.Publish(new OnApplicationStopStartEvent(this));
            Trace.WriteLine("Stop all modules...");
            StopAllModules(_data);
            Trace.WriteLine("Run Dispose tasks");
            AppEngine.Dispose();
            _eventPublisher.Publish(new OnApplicationStopFinishEvent(this));
            DisplayExitCounter();
        }

        protected virtual void StartAllModules(AppDomainLoadData data)
        {
            var modules = GetModulesOrderedBy(data, m => m.StartupOrder);
            modules.ForEachItem(m =>
            {
                DefaultOutput.WriteLine("Starting " + m.Type);
                try
                {
                    m.Module.Start();
                }
                catch (Exception ex)
                {
                    DefaultOutput.WriteLine(">>>>>> Fail to start module!");
                    throw ex;
                }
            });
        }

        protected virtual void StopAllModules(AppDomainLoadData data)
        {
            GetModulesOrderedBy(data, m => m.StopOrder)
                .ForEachItem(m =>
                {
                    DefaultOutput.WriteLine("Stopping " + m.Type);
                    m.Module.Stop();
                });
        }

        #region Fields

        private readonly string _appId;
        protected readonly IConfigManager ConfigManager;
        private AppDomainLoadData _data;
        private IEventPublisher _eventPublisher;

        #endregion

        #region ctors

        /// <summary>
        ///     Creates new instance of Saturn72App
        /// </summary>
        /// <param name="appId">application code</param>
        protected Saturn72AppBase(string appId)
            : this(appId, Core.Configuration.ConfigManager.Current)

        {
        }

        /// <summary>
        ///     Created new instance of Saturn72App
        /// </summary>
        /// <param name="appId">Application code</param>
        /// <param name="configManager">Config holder</param>
        protected Saturn72AppBase(string appId, IConfigManager configManager)
        {
            _appId = appId;
            ConfigManager = configManager;
        }

        #endregion

        #region Fields

        public abstract string Name { get; }
        public abstract IEnumerable<IAppVersion> Versions { get; }

        #endregion

        #region Utilities

        private void DisplayExitCounter()
        {
            Console.WriteLine(_appId + " will be closed in ");
            var counter = 5;
            do
            {
                Console.WriteLine(counter--);
                Thread.Sleep(1000);
            } while (counter > 0);
            Console.WriteLine("Closing...");
        }

        private IEnumerable<ModuleInstance> GetModulesOrderedBy(AppDomainLoadData data, Func<ModuleInstance, int> func)
        {
            return GetActiveModuleInstancesOnly(data)
                .OrderBy(func)
                .ToArray();
        }

        private IEnumerable<ModuleInstance> GetActiveModuleInstancesOnly(AppDomainLoadData data)
        {
            return data.ModuleInstances.Where(m => m.Active);
        }

        #endregion
    }
}