#region

using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;
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
            actual.ConfigLoader.ShouldEqual("Saturn72.Core.Configuration.XmlConfigLoader, Saturn72.Core");
            actual.ConfigLoaderData.ShouldBeNull();
            actual.ContainerManager.ShouldEqual(
                    "Saturn72.Module.Ioc.Autofac.AutofacIocContainerManager, Saturn72.Module.Ioc.Autofac");
            actual.EngineDriver.ShouldEqual("Saturn72.Core.Infrastructure.AppEngineDriver, Saturn72.Core");
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
            actual.ConfigLoader.ShouldEqual("containerManager, containerManagerNamespace");
            actual.ConfigLoaderData.Count.ShouldEqual(3);
            actual.ConfigLoaderData["Key1"].ShouldEqual("Value1");
            actual.ConfigLoaderData["Key2"].ShouldEqual("Value2");
            actual.ConfigLoaderData["Key3"].ShouldEqual("Value3");

            actual.ContainerManager.ShouldEqual("containerManager, containerManagerNamespace");
            actual.EngineDriver.ShouldEqual("engineDriver, engineDriverNamespace");

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