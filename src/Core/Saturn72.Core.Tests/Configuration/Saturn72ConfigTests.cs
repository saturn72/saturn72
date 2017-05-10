#region

using System;
using System.IO;
using NUnit.Framework;
using Shouldly;
using Saturn72.Core.Configuration;

#endregion

namespace Saturn72.Core.Tests.Configuration
{
    public class Saturn72ConfigTests
    {
        [Test]
        public void GetConfiguration_GetDefaultConfig()
        {
            var tmp = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

            SetAppConfig("Config\\EmptyAppConfig.config");

            var actual = Saturn72Config.GetConfiguration();
            actual.ConfigLoader.ShouldBe("Saturn72.Core.Configuration.XmlConfigLoader, Saturn72.Core");
            actual.ConfigLoaderData.ShouldBeNull();
            actual.ContainerManager.ShouldBe(
                    "Saturn72.Module.Ioc.Autofac.AutofacIocContainerManager, Saturn72.Module.Ioc.Autofac");
            actual.EngineDriver.ShouldBe("Saturn72.Core.Infrastructure.AppEngineDriver, Saturn72.Core");
            SetAppConfig(tmp);
        }

        [Test]
        [Ignore("non-deterministic test")]
        [Category("ignored")]
        public void GetConfiguration_ShouldReturnConfigSectionValues()
        {
            var tmp = AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();
            Console.WriteLine(tmp);

            SetAppConfig("Config\\NonEmptyAppConfig.config");

            var actual = Saturn72Config.GetConfiguration();
            actual.ConfigLoader.ShouldBe("containerManager, containerManagerNamespace");
            actual.ConfigLoaderData.Count.ShouldBe(3);
            actual.ConfigLoaderData["Key1"].ShouldBe("Value1");
            actual.ConfigLoaderData["Key2"].ShouldBe("Value2");
            actual.ConfigLoaderData["Key3"].ShouldBe("Value3");

            actual.ContainerManager.ShouldBe("containerManager, containerManagerNamespace");
            actual.EngineDriver.ShouldBe("engineDriver, engineDriverNamespace");

            SetAppConfig(tmp);
        }

        private static void SetAppConfig(string relativePath)
        {
            relativePath = relativePath.Contains(":\\")
                ? relativePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", relativePath);
        }
    }
}