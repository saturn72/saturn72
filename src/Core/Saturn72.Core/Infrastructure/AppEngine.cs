#region

using System.Runtime.CompilerServices;
using Saturn72.Core.Configuration;
using Saturn72.Core.Patterns;

#endregion

namespace Saturn72.Core.Infrastructure
{
    public class AppEngine
    {
        public static IAppEngineDriver Current
        {
            get
            {
                return Singleton<IAppEngineDriver>.Instance ?? (Singleton<IAppEngineDriver>.Instance = Initialize());
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IAppEngineDriver Initialize(bool forceRestart = false)
        {
            if (Singleton<IAppEngineDriver>.Instance == null || forceRestart)
            {
                var config = Saturn72Config.GetConfiguration();

                Singleton<IAppEngineDriver>.Instance = CreateEngineInstance(config);
                Singleton<IAppEngineDriver>.Instance.Initialize(config);
            }
            return Singleton<IAppEngineDriver>.Instance;
        }

        private static IAppEngineDriver CreateEngineInstance(Saturn72Config config)
        {
            return CommonHelper.CreateInstance<IAppEngineDriver>(config.EngineDriver);
        }
    }
}