#region

using System.Threading.Tasks;
using System.Web.Http;
using Saturn72.Common.WebApi.Models.Account;
using Saturn72.Core;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Account")]
    public class AccountController : Saturn72ApiControllerBase
    {
        #region ctor

        public AccountController(IUserRegistrationService userRegistrationService, IWorkContext workContext)
        {
            _userRegistrationService = userRegistrationService;
            _workContext = workContext;
        }

        #endregion

        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationApiModel model)
        {
            if (model.IsNull())
                return BadRequest("Missing or no data for registration");

            if (!ModelState.IsValid)
                return BadRequest(ConvertModelStateErrorsToKeyValuePair());

            var request = new UserRegistrationRequest(model.Username, model.Email, model.Password,
                PasswordFormat.Encrypted, _workContext.CurrentUserIpAddress);
            var response = await _userRegistrationService.RegisterAsync(request);

            if (response.Success)
                return Ok();

            return BadRequestResult(response.Errors);
        }

        #region Fields

        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IWorkContext _workContext;

        #endregion
    }
}