#region

using System.Runtime.CompilerServices;
using Saturn72.Core.ComponentModel;
using Saturn72.Core.Configuration;

#endregion

namespace Saturn72.Core.Infrastructure
{
    public class AppEngine
    {
        public static IAppEngineDriver Current
            => Singleton<IAppEngineDriver>.Instance ?? (Singleton<IAppEngineDriver>.Instance = Initialize());

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IAppEngineDriver Initialize(bool forceRestart = false)
        {
            if (Singleton<IAppEngineDriver>.Instance == null || forceRestart)
            {
                var config = Saturn72Config.GetConfiguration();

                Singleton<IAppEngineDriver>.Instance = CommonHelper.CreateInstance<IAppEngineDriver>(config.EngineDriver);
                Singleton<IAppEngineDriver>.Instance.Initialize(config);
            }
            return Singleton<IAppEngineDriver>.Instance;
        }
    }
}