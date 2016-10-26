#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Common.UI;
using Saturn72.Core.Caching;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.AppDomainManagement;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi.Services
{
    public class HtmlRenderingService : IHtmlRenderingService
    {
        #region Consts

        private const string HtmlContentKey = "HtmlContent.";

        #endregion

        #region Properties

        protected IDictionary<string, string> HtmlDictionary
        {
            get { return _cacheManager.Get(HtmlContentKey, 1440, LoadHtmlContent); }
        }

        #endregion

        public Task<string> GetHtmlContentAsync(string key)
        {
            return Task.Run(() => HtmlDictionary.GetValueOrDefault(key, ()=>null));
        }

        private static IDictionary<string, string> LoadHtmlContent()
        {
            var result = new Dictionary<string, string>();
            AppEngine.Current.Resolve<ITypeFinder>().FindClassesOfTypeAndRunMethod<IHtmlContent>(s =>
                result.Add(s.HtmlKey, s.Html));
            return result;
        }

        #region Fields

        private readonly ICacheManager _cacheManager;

        public HtmlRenderingService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        #endregion
    }
}