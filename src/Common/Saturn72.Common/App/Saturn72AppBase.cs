#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Saturn72.Core;
using Saturn72.Core.Configuration;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;
using CommonHelper = Saturn72.Core.CommonHelper;

#endregion

namespace Saturn72.Common.App
{
    public abstract class Saturn72AppBase : IApp
    {
        #region Fields

        protected readonly string AppId;
        protected readonly IConfigManager ConfigManager;

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
            AppId = appId;
            ConfigManager = configManager;
        }

        #endregion

        #region Fields

        public abstract string Name { get; }
        public abstract IEnumerable<IAppVersion> Versions { get; }

        #endregion

        public virtual void Start()
        {
            Trace.WriteLine("Start {0} application".AsFormat(AppId));

            var data = ConfigManager.AppDomainLoadData;
            Trace.WriteLine("Read configuration file data and load external assemblies...");
            AppDomainLoader.Load(data);

            Trace.WriteLine("Loading modules...");
            LoadAllModules(data);

            Trace.WriteLine("Start application engine...");
            AppEngine.Initialize(true);

            Trace.WriteLine("Start all modules...");
            StartAllModules(data);

            Trace.WriteLine("Press CTRL+C to quit application...");

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();

            Trace.WriteLine("Stop all modules...");
            StopAllModules(data);
            RunDisposeTasks();
            DisplayExitCounter();
        }

        protected virtual void RunDisposeTasks()
        {
            AppEngine.Current.Dispose();
        }


        protected virtual void StartAllModules(AppDomainLoadData data)
        {
            var modules = GetModulesOrderedBy(data, m => m.StartupOrder);
            modules.ForEachItem(m =>
            {
                Trace.WriteLine("Starting " + m.Type);
                try
                {
                    m.Module.Start();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(">>>>>> Fail to start module!");
                    throw ex;
                }
            });
        }

        protected virtual void StopAllModules(AppDomainLoadData data)
        {
            GetModulesOrderedBy(data, m => m.StopOrder)
                .ForEachItem(m =>
                {
                    Trace.WriteLine("Stopping " + m.Type);
                    m.Module.Stop();
                });
        }

        protected virtual void LoadAllModules(AppDomainLoadData data)
        {
            GetActiveModuleInstancesOnly(data).ForEachItem(mi =>
            {
                mi.SetModule(CommonHelper.CreateInstance<IModule>(mi.Type));
                mi.Module.Load();
            });
        }

        #region Utilities

        private void DisplayExitCounter()
        {
            Trace.WriteLine(AppId + " will be closed in ");
            var counter = 5;
            do
            {
                Trace.WriteLine(counter--);
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