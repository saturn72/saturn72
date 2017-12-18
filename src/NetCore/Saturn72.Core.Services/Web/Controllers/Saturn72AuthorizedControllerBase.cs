using Microsoft.AspNetCore.Authorization;

namespace Saturn72.Core.Services.Web.Controllers
{
    [Authorize]
    public class Saturn72AuthorizedControllerBase : Saturn72ControllerBase
    {

    }
}