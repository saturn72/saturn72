using System.Linq;
using System.Runtime.Caching;
using Saturn72.Core.Caching;

namespace Saturn72.Core.Tests.Caching
{
    public class TestMemoryCacheManager : MemoryCacheManager
    {
        internal ObjectCache TempCache => Cache;

        public void Flush()
        {
            foreach (var key in TempCache.Select(p => p.Key))
                TempCache.Remove(key);
        }
    }
}