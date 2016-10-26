#region

using System.Threading.Tasks;

#endregion

namespace Saturn72.Common.WebApi.Services
{
    public interface IHtmlRenderingService
    {
        Task<string> GetHtmlContentAsync(string key);
    }
}