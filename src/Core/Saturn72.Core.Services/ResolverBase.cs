#region

using System.Collections.Generic;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;

#endregion

namespace Saturn72.Core.Services
{
    public abstract class ResolverBase
    {
        private ITypeFinder _typeFinder;

        protected ITypeFinder TypeFinder
        {
            get { return _typeFinder ?? (_typeFinder = Resolve<ITypeFinder>()); }
        }

        protected virtual IEnumerable<TService> ResolveAll<TService>() where TService : class
        {
            return AppEngine.Current.ResolveAll<TService>();
        }

        protected virtual TService Resolve<TService>() where TService : class
        {
            return AppEngine.Current.Resolve<TService>();
        }
    }
}