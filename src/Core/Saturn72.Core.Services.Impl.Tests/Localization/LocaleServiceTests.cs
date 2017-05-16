using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Saturn72.Core.Caching;
using Saturn72.Core.Domain.Localization;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Impl.Localization;
using Saturn72.Core.Services.Localization;
using Saturn72.Extensions;
using Shouldly;

namespace Saturn72.Core.Services.Impl.Tests.Localization
{
    public class LocaleServiceTests
    {
        private static readonly IDictionary<string, object> _cachedObjects = new Dictionary<string, object>();

        private readonly IEnumerable<LocaleResourceDomainModel> _localeResources = new[]
        {
            new LocaleResourceDomainModel {Id = 1, Key = "Key1", Value = "Value1", LanguageId = 0},
            new LocaleResourceDomainModel {Id = 1, Key = "Key2", Value = "Value2", LanguageId = 0},
            new LocaleResourceDomainModel {Id = 1, Key = "Key3", Value = "Value3", LanguageId = 0},
            new LocaleResourceDomainModel {Id = 1, Key = "Key4", Value = "Value4", LanguageId = 0},
            new LocaleResourceDomainModel {Id = 1, Key = "Key2", Value = "Value2", LanguageId = 1},
            new LocaleResourceDomainModel {Id = 1, Key = "Key3", Value = "Value3", LanguageId = 1},
            new LocaleResourceDomainModel {Id = 1, Key = "Key4", Value = "Value4", LanguageId = 2}
        };

        private static ICacheManager BuildCacheManagerMock()
        {
            var cm = new Mock<ICacheManager>();
            cm.Setup(c => c.Keys)
                .Returns(new string[] {});

            cm.Setup(c => c.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback((string key, object o, int i) => _cachedObjects[key] = o);
            cm.Setup(c => c.Get<IEnumerable<LocaleResourceDomainModel>>(It.IsAny<string>()))
                .Returns((string key) => _cachedObjects[key] as IEnumerable<LocaleResourceDomainModel>);

            return cm.Object;
        }

        private ILocaleResourceRespository BuildLocaleResourceRepositoryMock()
        {
            var repo = new Mock<ILocaleResourceRespository>();
            repo.Setup(x => x.GetLocaleResource(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(
                    (string key) =>
                        _localeResources.FirstOrDefault(
                            lr => lr.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)));

            repo.Setup(x => x.GetAllLocaleResources(It.IsAny<int>()))
                .Returns((int langId) => _localeResources.Where(lr => lr.LanguageId == langId));

            return repo.Object;
        }

        [Test]
        public void GetResource_ReturnsNullOrEmptyOrDefaultValue_OnEmptyResourceKey()
        {
            var srv = new LocaleService(null, null, null);
            srv.GetLocaleResource("", 0).ShouldBe("");
            srv.GetLocaleResource(null, 0).ShouldBe("");
            var defaultVal = "ggg";
            srv.GetLocaleResource("", 0, defaultVal).ShouldBe(defaultVal);
            srv.GetLocaleResource(null, 0, defaultVal).ShouldBe(defaultVal);

            srv.GetLocaleResource("", 100, defaultVal, true).ShouldBeNull();
            srv.GetLocaleResource(null, 100, defaultVal, true).ShouldBeNull();
            srv.GetLocaleResource("", 100, returnNullOnNotFound: true).ShouldBeNull();
            srv.GetLocaleResource(null, 100, returnNullOnNotFound: true).ShouldBeNull();
        }

        [Test]
        public void LocaleService_GetResource_ReturnsNullOrEmptyOrDefaultValue_OnNotFoundResourceKey()
        {
            var _logger = new Mock<ILogger>();
            _logger.Setup(l => l.SupportedLogLevels).Returns(LogLevel.AllSystemLogLevels.ToArray());
            var loggerInsertions = 0;

            var _cacheManager = BuildCacheManagerMock();
            var _repo = BuildLocaleResourceRepositoryMock();

            var srv = new LocaleService(_repo, _cacheManager, _logger.Object);


            srv.GetLocaleResource("Key1ssssss", 0).ShouldBe("");
            _logger.Verify(
                l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Exactly(++loggerInsertions));
            srv.GetLocaleResource("Key1ssssss", 0, "defVal").ShouldBe("defVal");
            _logger.Verify(
                l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Exactly(++loggerInsertions));

            srv.GetLocaleResource("Key1ssssss", 0, "defVal", true).ShouldBeNull();

            _logger.Verify(
                l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Exactly(++loggerInsertions));


            srv.GetLocaleResource("Key1ssssss", 0).ShouldBe(string.Empty);
            _logger.Verify(
                l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Exactly(++loggerInsertions));

            srv.GetLocaleResource("Key1ssssss", 0, returnNullOnNotFound: true).ShouldBeNull();
            _logger.Verify(
                l => l.InsertLog(It.IsAny<LogLevel>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Exactly(++loggerInsertions));
        }

        [Test]
        public void LocaleService_GetResource_GetAllLocaleResources_CacheLocaleresources()
        {
            var cacheManager = BuildCacheManagerMock();
            var repo = BuildLocaleResourceRepositoryMock();

            var srv = new LocaleService(repo, cacheManager, null);
            var langId = 0;

            var res = srv.GetAllLocaleResources(langId).ToArray();

            var langLocaleResource = _localeResources.Where(lr => lr.LanguageId == langId);
            res.Length.ShouldBe(langLocaleResource.Count());
            for (var i = 0; i < langLocaleResource.Count(); i++)
            {
                var lr = langLocaleResource.ElementAt(i);
                res[i].Key.ShouldBe(lr.Key.ToLowerInvariant());
                res[i].Value.Key.ShouldBe(lr.Id);
                res[i].Value.Value.ShouldBe(lr.Value);
            }
        }

    }

    public class TestLocaleService : LocaleService
    {
        public TestLocaleService(ILocaleResourceRespository localeResourceRespository, ICacheManager cacheManager,
            ILogger logger) : base(localeResourceRespository, cacheManager, logger)
        {
        }

        public override string GetLocaleResource(string resourceKey, int languageId, string defaultValue = "",
            bool returnNullOnNotFound = false)
        {
            return resourceKey;
        }
    }
}