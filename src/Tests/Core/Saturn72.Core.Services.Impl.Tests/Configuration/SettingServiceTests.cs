#region

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Configuration;
using Saturn72.Core.Domain.Configuration;
using Saturn72.Core.Services.Configuration;
using Saturn72.Core.Services.Data.Repositories;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Impl.Configuration;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Core.Services.Impl.Tests.Configuration
{
    public class SettingServiceTests
    {
        [Test]
        public void SettingService_SavesSettings_throwsOnNull()
        {
            var cacheManager = new Mock<ICacheManager>();
            var repo = new Mock<ISettingEntryRepository>();
            var ePublisher = new Mock<IEventPublisher>();

            var service = new SettingsService(cacheManager.Object, repo.Object, ePublisher.Object);

            typeof(NullReferenceException).ShouldBeThrownBy(() => service.SaveSetting<TSettings1>(null));
        }

        [Test]
        public void SettingService_SavesSettings()
        {
            //Arrange
            var settings = new TSettings1
            {
                IntValue = 100,
                StringValue = "string1"
            };
            var settingList = new List<SettingEntryDomainModel>();

            var cacheManager = new Mock<ICacheManager>();
            var ePublisher = new Mock<IEventPublisher>();

            var repo = new Mock<ISettingEntryRepository>();
            repo.Setup(r => r.Create(It.IsAny<SettingEntryDomainModel>()))
                .Callback<SettingEntryDomainModel>(s =>
                {
                    s.Id = settingList.Count + 1;
                    settingList.Add(s);
                });

            repo.Setup(r => r.GetAll()).Returns(() => settingList);
            repo.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns<long>(id => settingList.FirstOrDefault(s => s.Id == id));

            ISettingsService service = new SettingsService(cacheManager.Object, repo.Object, ePublisher.Object);

            //Act
            service.SaveSetting(settings);

            //Assert
            settingList.Count.ShouldEqual(3);
            settingList.ElementAt(0).Name.ShouldEqual("TSettings1.IntValue".ToLower());
            settingList.ElementAt(0).Value.ShouldEqual(settings.IntValue.ToString());

            settingList.ElementAt(1).Name.ShouldEqual("TSettings1.StringValue".ToLower());
            settingList.ElementAt(1).Value.ShouldEqual(settings.StringValue);

            settingList.ElementAt(2).Name.ShouldEqual("TSettings1.Menu".ToLower());
            settingList.ElementAt(2).Value.ShouldEqual(string.Empty);

            //flow assertion
            cacheManager.Verify(c => c.RemoveByPattern(It.IsAny<string>()));
            ePublisher.Verify(e => e.Publish(It.IsAny<CreatedEvent<SettingEntryDomainModel, long>>()), Times.Exactly(3));
        }

        [Test]
        public void SettingService_UpdatesSettings()
        {
            //Arrange
            var settings = new TSettings1
            {
                IntValue = 100,
                StringValue = "string1"
            };
            var settingList = new List<SettingEntryDomainModel>();

            var cacheManager = new Mock<ICacheManager>();
            var ePublisher = new Mock<IEventPublisher>();

            var repo = new Mock<ISettingEntryRepository>();
            repo.Setup(r => r.Create(It.IsAny<SettingEntryDomainModel>()))
                .Callback<SettingEntryDomainModel>(s =>
                {
                    s.Id = settingList.Count + 1;
                    settingList.Add(s);
                });

            repo.Setup(r => r.GetAll()).Returns(() => settingList);
            repo.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns<long>(id => settingList.FirstOrDefault(s => s.Id == id));

            ISettingsService service = new SettingsService(cacheManager.Object, repo.Object, ePublisher.Object);

            //Act
            service.SaveSetting(settings);

            settings.IntValue = 101;
            settings.StringValue = "string2";
            service.SaveSetting(settings);

            //Assert
            settingList.Count.ShouldEqual(3);
            settingList.ElementAt(0).Name.ShouldEqual("TSettings1.IntValue".ToLower());
            settingList.ElementAt(0).Value.ShouldEqual(settings.IntValue.ToString());

            settingList.ElementAt(1).Name.ShouldEqual("TSettings1.StringValue".ToLower());
            settingList.ElementAt(1).Value.ShouldEqual(settings.StringValue.ToLower());

            settingList.ElementAt(2).Name.ShouldEqual("TSettings1.Menu".ToLower());
            settingList.ElementAt(2).Value.ShouldEqual(string.Empty);

            //flow assertion
            cacheManager.Verify(c => c.RemoveByPattern(It.IsAny<string>()));
            ePublisher.Verify(e => e.Publish(It.IsAny<CreatedEvent<SettingEntryDomainModel, long>>()), Times.Exactly(3));
            ePublisher.Verify(e => e.Publish(It.IsAny<UpdatedEvent<SettingEntryDomainModel, long>>()),Times.Exactly(3));
        }

        [Test]
        public void SettingService_DeletesSettings()
        {
            //Arrange
            var settings1 = new TSettings1
            {
                IntValue = 100,
                StringValue = "string1",
                Menu = "Bar"
            };

            var settings2 = new TSettings2
            {
                Numeric = 100,
                Text = "string1"
            };

            var settingList = new List<SettingEntryDomainModel>();

            var cacheManager = new Mock<ICacheManager>();
            var ePublisher = new Mock<IEventPublisher>();

            var repo = new Mock<ISettingEntryRepository>();
            repo.Setup(r => r.Create(It.IsAny<SettingEntryDomainModel>()))
                .Callback<SettingEntryDomainModel>(s =>
                {
                    s.Id = settingList.Count + 1;
                    settingList.Add(s);
                });

            repo.Setup(r => r.GetAll()).Returns(() => settingList);
            repo.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns<long>(id => settingList.FirstOrDefault(s => s.Id == id));

            ISettingsService service = new SettingsService(cacheManager.Object, repo.Object, ePublisher.Object);

            //Act
            service.SaveSetting(settings1);
            service.SaveSetting(settings2);
            service.DeleteSetting<TSettings1>();

            var allsettings = service.GetAllSettingEntries();
            allsettings.Count().ShouldEqual(5);

            var names = allsettings.Select(x => x.Name).ToList();
            names.ShouldContainInstance("TSettings2.Numeric".ToLower());
            names.ShouldContainInstance("TSettings2.Text".ToLower());

            var values = allsettings.Select(x => x.Value).ToList();
            values.ShouldContainInstance(settings2.Numeric.ToString());
            values.ShouldContainInstance(settings2.Text);

            //flow assertion
            ePublisher.Verify(e => e.Publish(It.IsAny<DeletedEvent<SettingEntryDomainModel, long>>()), Times.Exactly(3));
        }

        [Test]
        public void SettingService_GetAllSettingEntries()
        {
            //Arrange
            var settings1 = new TSettings1
            {
                IntValue = 100,
                StringValue = "string1",
                Menu = "Bar"
            };

            var settings2 = new TSettings2
            {
                Numeric = 100,
                Text = "string1"
            };

            var settingList = new List<SettingEntryDomainModel>();

            var cacheManager = new Mock<ICacheManager>();
            var ePublisher = new Mock<IEventPublisher>();

            var repo = new Mock<ISettingEntryRepository>();
            repo.Setup(r => r.Create(It.IsAny<SettingEntryDomainModel>()))
                .Callback<SettingEntryDomainModel>(s =>
                {
                    s.Id = settingList.Count + 1;
                    settingList.Add(s);
                });

            repo.Setup(r => r.GetAll()).Returns(() => settingList);
            repo.Setup(r => r.GetById(It.IsAny<long>()))
                .Returns<long>(id => settingList.FirstOrDefault(s => s.Id == id));

            ISettingsService service = new SettingsService(cacheManager.Object, repo.Object, ePublisher.Object);

            //Act
            service.SaveSetting(settings1);
            service.SaveSetting(settings2);

            var allsettings = service.GetAllSettingEntries();
            allsettings.Count().ShouldEqual(5);

            var names = allsettings.Select(x => x.Name).ToList();
            names.ShouldContainInstance("TSettings1.IntValue".ToLower());
            names.ShouldContainInstance("TSettings1.StringValue".ToLower());
            names.ShouldContainInstance("TSettings1.Menu".ToLower());
            names.ShouldContainInstance("TSettings2.Numeric".ToLower());
            names.ShouldContainInstance("TSettings2.Text".ToLower());

            var values = allsettings.Select(x => x.Value).ToList();
            values.ShouldContainInstance(settings1.IntValue.ToString());
            values.ShouldContainInstance(settings1.StringValue);
            values.ShouldContainInstance(settings1.Menu);
            values.ShouldContainInstance(settings2.Numeric.ToString());
            values.ShouldContainInstance(settings2.Text);

        }

        public class TSettings1 : SettingsBase
        {
            public int IntValue { get; set; }

            public string StringValue { get; set; }
            public string Menu { get; set; }
        }
    }

    public class TSettings2 : SettingsBase
    {
        public string Text { get; set; }
        public int Numeric { get; set; }
    }
}