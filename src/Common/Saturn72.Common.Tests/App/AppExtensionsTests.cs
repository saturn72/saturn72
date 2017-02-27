﻿
using Moq;
using NUnit.Framework;
using Saturn72.Common.App;
using Saturn72.UnitTesting.Framework;

namespace Saturn72.Common.Tests.App
{
    public class AppExtensionsTests
    {
        [Test]
        public void AppVersionExtensions_GetLatestVersion()
        {
            var v1 = new AppVersion1();
            var v2 = new AppVersion2();
            var app = new Mock<IApp>();
            app.Setup(a => a.Versions).Returns(new IAppVersion[] {v1, v2});

            app.Object.GetLatestVersion().ShouldEqual(v2);
        }

        internal class AppVersion1:IAppVersion
        {
            public string Key { get; }
            public int Index { get; }
            public bool IsLatest => false;
            public bool Publish { get; }
            public AppVersionStatusType Status { get; }
        }

        internal class AppVersion2 : IAppVersion
        {
            public string Key { get; }
            public int Index { get; }
            public bool IsLatest => true;
            public bool Publish { get; }
            public AppVersionStatusType Status { get; }
        }
    }
}
