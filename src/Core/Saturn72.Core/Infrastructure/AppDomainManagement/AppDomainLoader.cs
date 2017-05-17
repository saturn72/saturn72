#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
        private const string PluginDescriptionFile = "descriptor.json";
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();
        private static ICollection<PluginDescriptor> _pluginDescriptors;
        public static AppDomainLoadData AppDomainLoadData { get; private set; }

        public static IEnumerable<PluginDescriptor> PluginDescriptors => _pluginDescriptors;

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
            if (!DirectoryIsAccessibleAndHaveFilesOrDirectories(data.RootDirectory))
            {
                Trace.TraceWarning("No Modules were found in modules root directory or unaccessible directory: " +
                                   data.RootDirectory);
                return;
            }

            using (new WriteLockDisposable(Locker))
            {
                if (!PrepareFileSystemForPluginsOrModules(data))
                    return;

                DeployModulesDlls(data);
            }
        }

        private static void LoadAllPlugins(DynamicLoadingData data)
        {
            if (!DirectoryIsAccessibleAndHaveFilesOrDirectories(data.RootDirectory))
            {
                Trace.TraceWarning("No PLUGINS were found in root directory or unaccessible directory: " + data.RootDirectory);
                return;
            }
            using (new WriteLockDisposable(Locker))
            {
                if (!PrepareFileSystemForPluginsOrModules(data))
                    return;

                _pluginDescriptors = new List<PluginDescriptor>();
                var installedOrSuspendedPlugins = GetInstalledAndSuspendedPluginsSystemNames(data.ConfigFile);

                var pluginDescriptors = GetPluginDescriptors(data.RootDirectory);
                foreach (var pd in pluginDescriptors)
                {
                    ValidatePluginBySystemName(pd, _pluginDescriptors);
                    pd.State = GetPluginState(installedOrSuspendedPlugins, pd.TypeFullName);
                    _pluginDescriptors.Add(pd);
                }

                DeployPluginsDlls(data);
            }
        }

        private static bool DirectoryIsAccessibleAndHaveFilesOrDirectories(string directory)
        {
            try
            {
                return Directory.Exists(directory) && Directory.GetDirectories(directory).Any() ||
                       !Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Any();
            }
            catch (DirectoryNotFoundException e)
            {
                return false;
            }
        }

        private static PluginState GetPluginState(IEnumerable<PluginDescriptor> plugins, string typeFullName)
        {
            var plugin = plugins.FirstOrDefault(p => p.TypeFullName.EqualsTo(typeFullName));

            return plugin.NotNull() ? plugin.State : PluginState.Uninstalled;
        }

        private static void DeployPluginsDlls(DynamicLoadingData data)
        {
            var shadowCopyDirectory = data.ShadowCopyDirectory;
            var binFiles = Directory.Exists(shadowCopyDirectory)
                ? Directory.GetFiles(shadowCopyDirectory)
                : new string[] { };
            var pluginsToDeploy = _pluginDescriptors.Where(pd => pd.State != PluginState.Uninstalled);

            try
            {
                foreach (var pd in pluginsToDeploy)
                {
                    var dynamicLoadedFiles =
                        Directory.GetFiles(pd.DescriptorFile.DirectoryName, "*.dll", SearchOption.AllDirectories)
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
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var exception in ex.LoaderExceptions)
                    msg = msg + exception.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Trace.WriteLine(fail.Message);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            catch (Exception ex)
            {
                var msg = string.Empty;
                for (var e = ex; e != null; e = e.InnerException)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Trace.WriteLine(fail.Message);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
        }

        private static void DeployModulesDlls(DynamicLoadingData data)
        {
            var shadowCopyDirectory = data.ShadowCopyDirectory;
            var binFiles = Directory.Exists(shadowCopyDirectory)
                ? Directory.GetFiles(shadowCopyDirectory)
                : new string[] { };

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
                Trace.WriteLine(fail.Message);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
            catch (Exception ex)
            {
                var msg = string.Empty;
                for (var e = ex; e != null; e = e.InnerException)
                    msg += e.Message + Environment.NewLine;

                var fail = new Exception(msg, ex);
                Trace.WriteLine(fail.Message);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }
        }
        private static IEnumerable<PluginDescriptor> GetInstalledAndSuspendedPluginsSystemNames(string pluginConfigFile)
        {
            var installedPluginsFile = FileSystemUtil.RelativePathToAbsolutePath(pluginConfigFile);
            if (!FileSystemUtil.FileExists(installedPluginsFile))
                return new PluginDescriptor[] { };

            IEnumerable<PluginDescriptor> result;
            using (var jReader = new JsonTextReader(File.OpenText(installedPluginsFile)))
            {
                result = new JsonSerializer().Deserialize<PluginDescriptor[]>(jReader);
            }
            result.ForEachItem(s => s.TypeFullName = RemoveDuplicateWhiteSpaces(s.TypeFullName));

            var multipleEntries = result
                .GroupBy(pd => pd.TypeFullName.ToLower())
                .Where(c => c.Count() > 1);

            if (multipleEntries.Any())
            {
                var dupEntries = string.Join("; ",multipleEntries.Select(i=>i.Key));
                throw new InvalidOperationException("{0} contains multiple entries for the same plugin. Duplicate plugin types: {1}".AsFormat(pluginConfigFile, dupEntries));
            }
            return result;
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
                : new string[] { };

            DeleteShadowDirectoryIfRequired(data.DeleteShadowCopyOnStartup, shadowCopyDirectory,
                binFiles);

            FileSystemUtil.CreateDirectoryIfNotExists(shadowCopyDirectory);
            return true;
        }

        private static void ValidatePluginBySystemName(PluginDescriptor pluginDescriptor,
            IEnumerable<PluginDescriptor> referencedPlugins)
        {
            var typeFullName = pluginDescriptor.TypeFullName;

            Guard.HasValue(typeFullName,
                "A plugin '{0}' has no system name. Try assigning the plugin a unique name and recompiling.".AsFormat(
                    pluginDescriptor));

            Func<bool> pluginWasNotLoadedCondition = () => !referencedPlugins.Any(
                pd => pd.TypeFullName.Equals(typeFullName, StringComparison.InvariantCultureIgnoreCase));

            Guard.MustFollow(pluginWasNotLoadedCondition,
                "A plugin with '{0}' system name is already defined".AsFormat(typeFullName));
        }


        private static IEnumerable<PluginDescriptor> GetPluginDescriptors(string pluginFolder)
        {
            Guard.HasValue(pluginFolder);

            //create list (<file info, parsed plugin descritor>)
            var result = new List<PluginDescriptor>();
            var pluginDirInfo = new DirectoryInfo(pluginFolder);

            //add display order and path to list
            var descriptionFiles = pluginDirInfo.GetFiles(PluginDescriptionFile, SearchOption.AllDirectories);

            descriptionFiles.ForEachItem(pluginDescriptor =>
                result.Add(ParsePluginDescriptionFile(pluginDescriptor)));
            return result;
        }

        private static PluginDescriptor ParsePluginDescriptionFile(FileInfo pluginInfo)
        {
            using (var file = File.OpenText(pluginInfo.FullName))
            {
                var serializer = new JsonSerializer();

                var pluginDescriptor = (PluginDescriptor) serializer.Deserialize(file, typeof(PluginDescriptor));
                pluginDescriptor.TypeFullName = RemoveDuplicateWhiteSpaces(pluginDescriptor.TypeFullName);
                pluginDescriptor.DescriptorFile = pluginInfo;
                return pluginDescriptor;
            }
        }

        private static string RemoveDuplicateWhiteSpaces(string source)
        {
            return Regex.Replace(source, "[ ]{2,}", " ");
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
            Trace.WriteLine(asmFullName + " was loaded");
            //add the reference to the build manager
            if (CommonHelper.IsWebApp())
            {
                Trace.WriteLine("Adding to BuildManager: '{0}'", shadowCopyAssembly.FullName);
                BuildManager.AddReferencedAssembly(shadowCopyAssembly);
            }
        }

        private static void VerifyAssemblyIsNotAlreadyLoaded(string assemblyPath)
        {
            var allAppDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (allAppDomainAssemblies.Any(asm => GetAssemblyLocalPath(asm).Equals(assemblyPath)))
                throw new Saturn72Exception("{0} was already loaded to app domaim".AsFormat(assemblyPath));
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
                var message =
                    "The component directory for the {0} file exists in a folder outside of the allowed saturn72 folder heirarchy"
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