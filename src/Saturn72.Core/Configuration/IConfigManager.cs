using System;

namespace Saturn72.Core.Configuration
{
    public interface IConfigManager
    {
        object GetConfig(Type configType);
    }
}
