#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;

#endregion

namespace Saturn72.Module.Ioc.Autofac.Owin
{
    public class AutofacOwinModule : IModule
    {
        public void Load(IDictionary<string, IConfigMap> configurations)
        {
        }

        public void Start(IDictionary<string, IConfigMap> configuration)
        {
        }

        public void Stop(IDictionary<string, IConfigMap> configurations)
        {
        }
    }
}