using System.Collections.Generic;
using System.Linq;

namespace Saturn72.Module.EmailClients.Domain.Filters
{
    public static class ImapFilterExtensions
    {
        public static IEnumerable<TFilter> Filter<TFilter>(this IImapFilter<TFilter> filter,
            IEnumerable<TFilter> collection)
        {
            return collection.Where(filter.Filter).ToArray();
        }
    }
}