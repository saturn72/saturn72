#region

using System.Collections.Generic;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Tasks;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;

#endregion

namespace Saturn72.Module.HangFire
{
    public class HangFireModule : IModule
    {
        public void Load()
        {
        }

        public void Start()
        {
            AppEngine.Current.Resolve<ITaskManager>().Initialize();
        }

        public void Stop()
        {
        }
    }
}