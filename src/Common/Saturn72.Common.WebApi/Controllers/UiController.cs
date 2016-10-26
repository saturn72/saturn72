using System.Threading.Tasks;
using System.Web.Http;
using Saturn72.Common.WebApi.Services;

namespace Saturn72.Common.WebApi.Controllers
{
    public class UiController : Saturn72ApiControllerBase
    {
        private readonly IHtmlRenderingService _htmlRenderingService;

        public UiController(IHtmlRenderingService htmlRenderingService)
        {
            _htmlRenderingService = htmlRenderingService;
        }

        [HttpGet]
        public async Task<string> GetByName(string id)
        {
            return await _htmlRenderingService.GetHtmlContentAsync(id);
        }
    }
}