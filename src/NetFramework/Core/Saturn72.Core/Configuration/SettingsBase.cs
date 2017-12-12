#region

using System.Collections.Generic;
using Saturn72.Core.Domain.Configuration;

#endregion

namespace Saturn72.Core.Configuration
{
    public abstract class SettingsBase
    {
        private IDictionary<string, SettingEntryModel> _settingEntries;

        public IDictionary<string, SettingEntryModel> SettingEntries
        {
            get { return _settingEntries ?? (_settingEntries = new Dictionary<string, SettingEntryModel>()); }
        }
    }
}