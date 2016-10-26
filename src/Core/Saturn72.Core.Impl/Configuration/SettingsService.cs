#region

using System;
using System.Collections.Generic;
using System.Linq;
using Saturn72.Core.Caching;
using Saturn72.Core.Configuration;
using Saturn72.Core.Domain.Configuration;
using Saturn72.Core.Services.Configuration;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Services.Impl.Configuration
{
    public class SettingsService : ISettingsService
    {
        #region Ctor

        public SettingsService(ICacheManager cacheManager, ISettingEntryRepository settingEntryRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _settingEntryRepository = settingEntryRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        public TSettings LoadSettings<TSettings>() where TSettings : SettingsBase
        {
            var settings = Activator.CreateInstance<TSettings>();

            foreach (var prop in typeof(TSettings).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = typeof(TSettings).Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key);
                if (setting == null)
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = CommonHelper.GetCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting);
                Guard.NotNull(value);
                //set property
                prop.SetValue(settings, value, null);
                settings.SettingEntries.Add(prop.Name,
                    new SettingEntryDomainModel {Name = prop.Name, Value = value.ToString()});
            }

            return settings;
        }

        public void DeleteSetting<TSettings>() where TSettings : SettingsBase, new()
        {
            var settingsToDelete = new List<SettingEntryDomainModel>();
            var allSettings = GetAllSettingEntries();
            foreach (var prop in typeof(TSettings).GetProperties())
            {
                var settingKey = typeof(TSettings).Name + "." + prop.Name;
                settingsToDelete.AddRange(
                    allSettings.Where(x => x.Name.Equals(settingKey, StringComparison.InvariantCultureIgnoreCase)));
            }

            DeleteSettingEntries(settingsToDelete);
        }

        public void DeleteSettingEntries(IEnumerable<SettingEntryDomainModel> settingEntries)
        {
            if (settingEntries.IsEmptyOrNull())
                return;

            _settingEntryRepository.Delete(settingEntries);

            ClearCache();

            settingEntries.ForEachItem(_eventPublisher.DomainModelDeleted<SettingEntryDomainModel, long>);
        }


        public virtual IEnumerable<SettingEntryDomainModel> GetAllSettingEntries()
        {
            return _settingEntryRepository.GetAll().OrderBy(s => s.Name).ToArray();
        }

        public void SaveSetting<TSettings>(TSettings settings) where TSettings : SettingsBase, new()
        {
            Guard.NotNull(settings);

            foreach (var prop in typeof(TSettings).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(TSettings).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(key, value, false);
                else
                    SetSetting(key, "", false);
            }

            ClearCache();
        }

        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (!key.HasValue())
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (settings.ContainsKey(key))
            {
                var settingsByKey = settings[key];
                var setting = settingsByKey.FirstOrDefault();

                //load shared value?
                if (setting.IsNull())
                    setting = settingsByKey.FirstOrDefault();

                if (setting != null)
                    return CommonHelper.To<T>(setting.Value);
            }

            return defaultValue;
        }

        #region Nested Classes

        [Serializable]
        public class SettingForCaching
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

        #endregion

        #region Utilities

        public virtual void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            Guard.NotNull(key);

            key = key.Trim().ToLowerInvariant();
            var valueStr = CommonHelper.GetCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key)
                ? allSettings[key].FirstOrDefault()
                : null;

            if (settingForCaching.NotNull())
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSettingEntry(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new SettingEntryDomainModel
                {
                    Name = key,
                    Value = valueStr
                };
                InsertSetting(setting, clearCache);
            }
        }

        public virtual void InsertSetting(SettingEntryDomainModel setting, bool clearCache)
        {
            Guard.NotNull(setting);

            _settingEntryRepository.Create(setting);

            //cache
            if (clearCache)
                ClearCache();

            //event notification
            _eventPublisher.DomainModelCreated<SettingEntryDomainModel, long>(setting);
        }

        public virtual void UpdateSettingEntry(SettingEntryDomainModel settingEntry, bool clearCache = true)
        {
            Guard.NotNull(settingEntry);
            _settingEntryRepository.Update(settingEntry);

            if (clearCache)
                ClearCache();

            _eventPublisher.DomainModelUpdated<SettingEntryDomainModel, long>(settingEntry);
        }


        public virtual SettingEntryDomainModel GetSettingById(long settingId)
        {
            return settingId == 0
                ? null
                : _settingEntryRepository.GetById(settingId);
        }


        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(SettingsPatternKey);
        }


        protected virtual IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            //cache
            var key = string.Format(SettingsAllKey);
            return _cacheManager.Get(key, () =>
            {
                //we use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from s in _settingEntryRepository.GetAll()
                    orderby s.Name
                    select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        //first setting
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }
                return dictionary;
            });
        }

        #endregion

        #region Consts

        private const string SettingsAllKey = "Saturn72.setting.all";
        private const string SettingsPatternKey = "Saturn72.setting.";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly ISettingEntryRepository _settingEntryRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion
    }
}