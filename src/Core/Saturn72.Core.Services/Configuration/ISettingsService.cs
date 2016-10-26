#region

using System.Collections.Generic;
using Saturn72.Core.Configuration;
using Saturn72.Core.Domain.Configuration;

#endregion

namespace Saturn72.Core.Services.Configuration
{
    public interface ISettingsService
    {
        TSettings LoadSettings<TSettings>() where TSettings : SettingsBase;
        void DeleteSetting<TSettings>() where TSettings : SettingsBase, new();

        IEnumerable<SettingEntryDomainModel> GetAllSettingEntries();
        void SaveSetting<TSettings>(TSettings settings) where TSettings : SettingsBase, new();
    }
}