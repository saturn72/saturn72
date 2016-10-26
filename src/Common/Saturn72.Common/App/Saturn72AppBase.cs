#region

using System;
using System.Collections.Generic;
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
        private readonly IConfigManager _configManager;

        #endregion

        #region ctors

        /// <summary>
        ///     Creates new insta nce of Saturn72App
        /// </summary>
        /// <param name="appId">application code</param>
        protected Saturn72AppBase(string appId) : this(appId, "Config/ConfigRoot.xml")
        {
        }

        /// <summary>
        ///     Creates new instance of Saturn72App
        /// </summary>
        /// <param name="appId">application code</param>
        /// <param name="configRootPath">Root config file path</param>
        protected Saturn72AppBase(string appId, string configRootPath)
            : this(appId, ConfigManager.Current)

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
            _configManager = configManager;
        }

        #endregion

        #region Fields

        public abstract string Name { get; }
        public abstract IEnumerable<IAppVersion> Versions { get; }
        public abstract IAppVersion LatestVersion { get; }

        #endregion

        public virtual void Start()
        {
            Console.Out.WriteLine("Start {0} application".AsFormat(AppId));

            var data = _configManager.AppDomainLoadData;
            Console.Out.WriteLine("Read configuration file data and load external assemblies...");
            AppDomainLoader.Load(data);

            Console.Out.WriteLine("Start application engine...");
            AppEngine.Initialize(true);

            Console.Out.WriteLine("Loading modules...");
            LoadAllModules(data);

            Console.Out.WriteLine("Start all modules...");
            StartAllModules(data);

            Console.WriteLine("Press CTRL+C to quit application...");

            var exitEvent = new ManualResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            exitEvent.WaitOne();

            Console.Out.WriteLine("Stop all modules...");
            StopAllModules(data);
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
                    m.Module.Start(_configManager.ConfigMaps);
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
                    m.Module.Stop(_configManager.ConfigMaps);
                });
        }

        protected virtual void LoadAllModules(AppDomainLoadData data)
        {
            GetActiveModuleInstancesOnly(data).ForEachItem(mi =>
            {
                mi.SetModule(CommonHelper.CreateInstance<IModule>(mi.Type));
                mi.Module.Load(_configManager.ConfigMaps);
            });
        }

        #region Utilities

        private void DisplayExitCounter()
        {
            Console.WriteLine(AppId + " will be closed in ");
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
            var res = data.ModuleInstances.Where(m => m.Active);

            return res;
        }

        #endregion
    }
}