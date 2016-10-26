#region

using Saturn72.Core.Logging;

#endregion

namespace Saturn72.Core.Services
{
    public class Resolver : ResolverBase
    {
        private ILogger _logger;

        protected ILogger Logger
        {
            get { return _logger ?? (_logger = Resolve<ILogger>()); }
        }
    }
}