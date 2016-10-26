    using System;
    using System.Collections.Generic;
    using Saturn72.Common.App;
    using Saturn72.Core.Configuration;

    namespace HelloWorld
    {
        public class HelloWorldApp : Saturn72AppBase
        {
            private readonly IAppVersion _version = new AppVersion();
            public HelloWorldApp(string appId) : base(appId)
            {
            }

            public HelloWorldApp(string appId, string configRootPath) : base(appId, configRootPath)
            {
            }

            public HelloWorldApp(string appId, IConfigManager configManager) : base(appId, configManager)
            {
            }

            public override void Start()
            {
                Console.WriteLine("Hello from " + AppId + "application");
                Console.WriteLine("Nice to meet you");
                base.Start();
            }

            public override string Name => "HelloWorldApp";
            public override IEnumerable<IAppVersion> Versions => new [] {_version};
            public override IAppVersion LatestVersion => _version;
        }

        public class AppVersion:IAppVersion
        {
            public string Key => "version_key";
            public int Index => 0;
            public bool IsLatest => true;
            public bool Publish => true;
            public AppVersionStatus Status=> AppVersionStatus.Alpha;
        }
    }