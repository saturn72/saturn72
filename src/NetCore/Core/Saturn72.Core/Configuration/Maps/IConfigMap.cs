using System.Collections.Generic;

namespace Saturn72.Core.Configuration.Maps
{
    public interface IConfigMap
    {
        IDictionary<string, object> AllConfigRecords { get; }
        object GetValue(string key);
    }
}