#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Saturn72.Core;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Core.Services.Extensibility;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.Extensibility
{
    public class PluginManager : IPluginManager
    {
        public const string PluginDirectory = @"Plugins";
        private static readonly object LockObject = new object();
        private ICollection<PluginDescriptor> _allPluginDescriptors;

        public ICollection<PluginDescriptor> AllPlugins
        {
            get
            {
                return _allPluginDescriptors ?? (_allPluginDescriptors = AppDomainLoader.PluginDescriptors.ToList());
            }
        }


        public void UpdatePluginDescriptor(PluginDescriptor pluginDescriptor, PluginState newState)
        {
            var instance = pluginDescriptor.Instance();
            lock (LockObject)
            {
                ModifyPluginState(newState, instance);
                pluginDescriptor.State = newState;
                UpdateInstalledPluginFile();
            }
        }

        public IEnumerable<PluginDescriptor> GetAll()
        {
            return AllPlugins;
        }

        public PluginDescriptor GetBySystemName(string systemName)
        {
            return AllPlugins.FirstOrDefault(p => p.SystemName.EqualsTo(systemName));
        }

        private static void ModifyPluginState(PluginState newState, IPlugin instance)
        {
            switch (newState)
            {
                case PluginState.Installed:
                    instance.Install();
                    return;
                case PluginState.Uninstalled:
                    instance.Uninstall();
                    return;
                case PluginState.Suspended:
                    instance.Suspend();
                    return;
                case PluginState.Activated:
                    instance.Activate();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateInstalledPluginFile()
        {
            var format = "\t{{ \"typeFullName\": \"{0}\", \"systemName\": \"{1}\", \"state\": \"{2}\" }}";

            var sb = new StringBuilder();
            sb.AppendLine("[");
            AllPlugins.Where(p => p.State != PluginState.Uninstalled)
                .ForEachItem(p => sb.AppendLine(format.AsFormat(p.TypeFullName, p.SystemName, p.State)));
            sb.Append("]");

            var temp = Path.GetTempFileName();
            var tempBackup = Path.GetTempFileName();
            var installedPluginsFile = FileSystemUtil.RelativePathToAbsolutePath(AppDomainLoader.InstalledPluginsFile);

            try
            {
                File.WriteAllText(temp, sb.ToString());
                File.Replace(temp, installedPluginsFile, tempBackup);
            }
            catch (Exception ex)
            {
                DefaultOutput.WriteLine(ex.Message);
                DefaultOutput.WriteLine(ex);
            }
            finally
            {
                File.Delete(temp);
                File.Delete(tempBackup);
            }
        }
    }
}