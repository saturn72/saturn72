#region

using System.IO;
using Saturn72.Core.Configuration;
using Saturn72.Core.Extensibility;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Services.Configuration;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.Extensibility
{
    public abstract class PluginBase<TSettings> : IPlugin where TSettings : SettingsBase, new()
    {
        private string _absoluteFileSystemPath;
        private string _pluginName;
        private string _relativeFileSystemPath;
        private TSettings _settings;
        protected ISettingsService SettingsService;

        protected PluginBase()
        {
            SettingsService = AppEngine.Current.Resolve<ISettingsService>();
        }

        protected TSettings Settings
        {
            get { return _settings ?? (_settings = SettingsService.LoadSettings<TSettings>()); }
        }

        protected string PluginName
        {
            get { return _pluginName ?? (_pluginName = GetType().Assembly.GetName().Name); }
        }

        public virtual string RelativeFileSystemPath
        {
            get
            {
                return _relativeFileSystemPath ??
                       (_relativeFileSystemPath = Path.Combine(PluginManager.PluginDirectory, PluginName));
            }
        }

        public virtual string AbsoluteFileSystemPath
        {
            get
            {
                return _absoluteFileSystemPath ??
                       (_absoluteFileSystemPath = FileSystemUtil.RelativePathToAbsolutePath(RelativeFileSystemPath));
            }
        }

        public virtual void Install()
        {
            InstallSettings();
        }

        public virtual void Uninstall()
        {
            UninstallSettings();
        }

        #region Abstracts

        public abstract TSettings DefaultSettings { get; }

        public abstract void Suspend();

        public abstract void Activate();

        #endregion

        #region Utilities

        protected virtual void InstallSettings()
        {
            SettingsService.SaveSetting(DefaultSettings);
        }

        protected virtual void UninstallSettings()
        {
            SettingsService.DeleteSetting<TSettings>();
        }

        #endregion
    }
}