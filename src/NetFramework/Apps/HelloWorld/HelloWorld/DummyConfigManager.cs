using System;
using System.Collections.Generic;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure.AppDomainManagement;

namespace HelloWorld
{
    public class DummyConfigManager:IConfigManager
    {
        public IDictionary<string, Lazy<IConfigMap>> ConfigMaps { get; }
        public AppDomainLoadData AppDomainLoadData { get; }
    }
}
