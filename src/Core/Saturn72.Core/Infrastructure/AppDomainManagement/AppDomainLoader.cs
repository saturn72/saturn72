#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using Newtonsoft.Json;
using Saturn72.Core.ComponentModel;
using Saturn72.Core.Exceptions;
using Saturn72.Core.Extensibility;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Infrastructure.AppDomainManagement
{
    public class AppDomainLoader
    {
        private const string PluginDescriptionFile = "description.json";
        public const string InstalledPluginsFile = "App_Data\\InstalledPlugins.json";
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static ICollection<PluginDescriptor> _pluginDescriptors;
        public static AppDomainLoadData AppDomainLoadData { get; private set; }
        public static IEnumerable<PluginDescriptor> PluginDescriptors
        {
            get { return _pluginDescriptors; }
        }

        /// <summary>
        ///     Loads all components to AppDomain
        /// </summary>
        /// <param name="data">AppDomainLoadData <see cref="AppDomainLoadData" /></param>
        public static void Load(AppDomainLoadData data)
        {
            AppDomainLoadData = data;
            LoadExternalAssemblies(data.Assemblies);
            LoadAllModules(data.ModulesDynamicLoadingData);
            LoadAllPlugins(data.PluginsDynamicLoadingData);
        }

        private static void LoadExternalAssemblies(string[] assemblies)
        {
            if (assemblies.IsNull() || !assemblies.Any())
                return;

            //load all other referenced assemblies now
            var assembliesToBeLoaded = assemblies
                .Where(x => x.HasValue() && !IsAlreadyLoaded(x));

            if (!assembliesToBeLoaded.Any())
                return;

            var shadowCopyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            FileSystemUtil.CreateDirectoryIfNotExists(shadowCopyDirectory);
            foreach (var asm in assembliesToBeLoaded)
            {
                Guard.NotEmpty(asm);
                PerformFileDeploy(new FileInfo(asm), shadowCopyDirectory);
            }
        }

        private static void LoadAllModules(DynamicLoadingData data)
        {
            using (new WriteLockDisposable(Locker))
            {
                if (!PrepareFileSystemForPluginsOrModules(data))
                    return;

                DeployPluginOrModuleDlls(data);
            }
        }

        private static void LoadAllPlugins(DynamicLoadingData data)
        {
            using (new WriteLockDisposable(Locker))
            {
                if (!PrepareFileSystemForPluginsOrModules(data))
                    return;

                _pluginDescriptors = new List<PluginDescriptor>();
                var installedOrSuspendedPlugins = GetInstalledAndSuspendedPluginsSystemNames();

                foreach (var dfd in GetDescriptionFilesAndDescriptors(data.RootDirectory))
                {
                    var descriptionFile = dfd.Key;
                    var pluginDescriptor = dfd.Value;
                    Guard.NotNull(new object[] {descriptionFile, pluginDescriptor});

                    ValidatePluginBySystemName(dfd, _pluginDescriptors);

                    pluginDescriptor.State = GetPluginState(installedOrSuspendedPlugins, pluginDescriptor.SystemName);
                    
                    _pluginDescriptors.Add(pluginDescriptor);
                }

                DeployPluginOrModuleDlls(data);
            }
        }

        private static PluginState GetPluginState(IEnumerable<PluginDescriptor> plugins, string systemName)
        {
            var result = PluginState.Uninstalled;

            var plugin = plugins.FirstOrDefault(p => p.SystemName.EqualsTo(systemName));
            return plugin.IsNull() || !Enum.TryParse(plugin.State.ToString(), true, out result)
                ? result
                : result;
        }

        private static void DeployPluginOrModuleDlls(DynamicLoadingData data)
        {
            var shadowCopyDirectory = data.ShadowCopyDirectory;
            var binFiles = Directory.Exists(shadowCopyDirectory)
                ? Directory.GetFiles(shadowCopyDirectory)
                : new string[] {};

            try
            {
                var dynamicLoadedFiles =
                    Directory.GetFiles(data.RootDirectory, "*.dll", SearchOption.AllDirectories)
                        //Not in the shadow copy
                        .Where(x => !binFiles.Contains(x))
                        .ToArray();

                //load all other referenced assemblies now
                foreach (var dlf in dynamicLoadedFiles
                    .Where(x => !IsAlreadyLoaded(x)))
                {
                    Guard.NotEmpty(dlf);
                    PerformFileDeploy(new FileInfo(dlf), shadowCopyDirectory);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var exception in ex.LoaderExceptions)
                    msg = msg + exception.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            catch (Exception ex)
            {
                var msg = string.Empty;
                for (var e = ex; e != null; e = e.InnerException)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
        }

        private static IEnumerable<PluginDescriptor> GetInstalledAndSuspendedPluginsSystemNames()
        {
            var installedPluginsFile = FileSystemUtil.RelativePathToAbsolutePath(InstalledPluginsFile);
            if (!FileSystemUtil.FileExists(installedPluginsFile))
                return new PluginDescriptor[] {};

            using (var jReader = new JsonTextReader(File.OpenText(installedPluginsFile)))
            {
                return new JsonSerializer().Deserialize<PluginDescriptor[]>(jReader);
            }
        }

        private static bool PrepareFileSystemForPluginsOrModules(DynamicLoadingData data)
        {
            var rootDir = data.RootDirectory;
            Guard.NotEmpty(rootDir);
            if (!Directory.Exists(rootDir))
                return false;

            var shadowCopyDirectory = data.ShadowCopyDirectory;
            var binFiles = Directory.Exists(shadowCopyDirectory)
                ? Directory.GetFiles(shadowCopyDirectory)
                : new string[] {};

            DeleteShadowDirectoryIfRequired(data.DeleteShadowCopyOnStartup, shadowCopyDirectory,
                binFiles);

            FileSystemUtil.CreateDirectoryIfNotExists(shadowCopyDirectory);
            return true;
        }

        private static void ValidatePluginBySystemName(KeyValuePair<FileInfo, PluginDescriptor> dfd,
            IEnumerable<PluginDescriptor> referencedPlugins)
        {
            var pluginDescriptor = dfd.Value;
            var systemName = pluginDescriptor.SystemName;

            Guard.HasValue(systemName,
                "A plugin '{0}' has no system name. Try assigning the plugin a unique name and recompiling.".AsFormat(
                    dfd.Key));

            Func<bool> mustCondition = () => !referencedPlugins.Any(
                pd => pd.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));

            Guard.MustFollow(mustCondition,
                "A plugin with '{0}' system name is already defined".AsFormat(systemName));
        }


        private static IEnumerable<KeyValuePair<FileInfo, PluginDescriptor>> GetDescriptionFilesAndDescriptors(
            string pluginFolder)
        {
            Guard.HasValue(pluginFolder);

            //create list (<file info, parsed plugin descritor>)
            var result = new List<KeyValuePair<FileInfo, PluginDescriptor>>();
            var pluginDirInfo = new DirectoryInfo(pluginFolder);

            //add display order and path to list
            var descriptionFiles = pluginDirInfo.GetFiles(PluginDescriptionFile, SearchOption.AllDirectories);

            descriptionFiles.ForEachItem(d =>
                result.Add(new KeyValuePair<FileInfo, PluginDescriptor>(d, ParsePluginDescriptionFile(d.FullName))));
            return result;
        }

        private static PluginDescriptor ParsePluginDescriptionFile(string descriptionFile)
        {
            using (var file = File.OpenText(descriptionFile))
            {
                var serializer = new JsonSerializer();
                return (PluginDescriptor) serializer.Deserialize(file, typeof(PluginDescriptor));
            }
        }

        private static void DeleteShadowDirectoryIfRequired(bool shouldDeleteShadowCopyDirectories,
            string shadowCopyDirectory, string[] binFiles)
        {
            if (shouldDeleteShadowCopyDirectories && Directory.Exists(shadowCopyDirectory))
            {
                foreach (var f in binFiles)
                {
                    Debug.WriteLine("Deleting " + f);
                    try
                    {
                        FileSystemUtil.DeleteFile(f);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine("Error deleting file " + f + ". Exception: " + exc);
                    }
                }
                Thread.Sleep(150);
                FileSystemUtil.DeleteDirectory(shadowCopyDirectory);
            }
        }

        private static bool IsAlreadyLoaded(string file)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.TypeFullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.TypeFullName.Equals(fileAssemblyName.Module, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                if (!fileNameWithoutExt.HasValue())
                    throw new Exception(string.Format("Cannot get file extnension for {0}", file));
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyName = a.FullName.Split(',').FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + exc);
            }
            return false;
        }

        /// <summary>
        ///     Perform file deply
        /// </summary>
        /// <param name="component">Plugin file info</param>
        /// <param name="shadowCopyDirectory"></param>
        /// <returns>Assembly</returns>
        private static void PerformFileDeploy(FileInfo component, string shadowCopyDirectory)
        {
            VerifyNotNullComponent(component);
            //we can now register the plugin definition
            //First register using given path, if fail let .Net to figure how to load the assembly
            var asmFullName = GetDeploymentPathInfo(component, shadowCopyDirectory).FullName;
            VerifyAssemblyIsNotAlreadyLoaded(asmFullName);

            var shadowCopyAssembly = Assembly.Load(AssemblyName.GetAssemblyName(asmFullName));

            //add the reference to the build manager
            if (CommonHelper.IsWebApp())
            {
                Debug.WriteLine("Adding to BuildManager: '{0}'", shadowCopyAssembly.FullName);
                BuildManager.AddReferencedAssembly(shadowCopyAssembly);
            }
        }

        private static void VerifyAssemblyIsNotAlreadyLoaded(string assemblyPath)
        {
            var allAppDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (allAppDomainAssemblies.Any(asm => GetAssemblyLocalPath(asm).Equals(assemblyPath)))
            {
                throw new Saturn72Exception("{0} was already loaded to app domaim".AsFormat(assemblyPath));
            }
        }

        private static string GetAssemblyLocalPath(Assembly asm)
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return uri.LocalPath;
        }


        private static void VerifyNotNullComponent(FileInfo componentFileInfo)
        {
            Guard.NotNull(componentFileInfo);
            Guard.NotNull(componentFileInfo.Directory);
            Guard.NotNull(componentFileInfo.Directory.Parent, () =>
            {
                var message = "The component directory for the {0} file exists in a folder outside of the allowed saturn72 folder heirarchy"
                    .AsFormat(componentFileInfo.Name);
                throw new InvalidOperationException(message);
            });
        }

        private static FileInfo GetDeploymentPathInfo(FileInfo plugin, string shadowCopyDirectory)
        {
            if (CommonHelper.IsWebApp() && CommonHelper.GetTrustLevel() == AspNetHostingPermissionLevel.Unrestricted)
            {
                var directory = AppDomain.CurrentDomain.DynamicDirectory;
                Debug.WriteLine(plugin.FullName + " to " + directory);

                //were running in full trust so copy to standard dynamic folder
                return GetFullTrustDeploymentPath(plugin, new DirectoryInfo(directory));
            }

            return GetMediumTrustDeploymentPath(plugin, shadowCopyDirectory);
        }

        private static FileInfo GetMediumTrustDeploymentPath(FileSystemInfo component, string shadowCopyDirPath)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyDirPath, component.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= component.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Debug.WriteLine("Not copying; files appear identical: '{0}'", shadowCopiedPlug.Name);
                    shouldCopy = false;
                }
                else
                {
                    //delete an existing file

                    //More info: http://www.nopcommerce.com/boards/t/11511/access-error-nopplugindiscountrulesbillingcountrydll.aspx?p=4#60838
                    Debug.WriteLine("New plugin found; Deleting the old file: '{0}'", shadowCopiedPlug.Name);
                    File.Delete(shadowCopiedPlug.FullName);
                }
            }

            if (shouldCopy)
            {
                try
                {
                    File.Copy(component.FullName, shadowCopiedPlug.FullName, true);
                }
                catch (IOException)
                {
                    Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                    //this occurs when the files are locked,
                    //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                    //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                    try
                    {
                        var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                        File.Move(shadowCopiedPlug.FullName, oldFile);
                    }
                    catch (IOException exc)
                    {
                        throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin",
                            exc);
                    }
                    //ok, we've made it this far, now retry the shadow copy
                    File.Copy(component.FullName, shadowCopiedPlug.FullName, true);
                }
            }

            return shadowCopiedPlug;
        }

        private static FileInfo GetFullTrustDeploymentPath(FileSystemInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Debug.WriteLine(shadowCopiedPlug.FullName + " is locked, attempting to rename");
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            return shadowCopiedPlug;
        }
    }
}