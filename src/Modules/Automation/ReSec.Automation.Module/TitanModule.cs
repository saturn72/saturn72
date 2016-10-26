#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Extensibility;

#endregion

namespace Titan.Module
{
    public class TitanModule : IModule
    {
        public void Load(IDictionary<string, IConfigMap> configurations)
        {
            throw new NotImplementedException();
        }

        public void Start(IDictionary<string, IConfigMap> configuration)
        {
        }

        public void Stop(IDictionary<string, IConfigMap> configurations)
        {
        }
    }
}