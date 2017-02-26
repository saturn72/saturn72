#region

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Saturn72.Common.WebApi.Models.Account;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.User;

#endregion

namespace Saturn72.Common.WebApi.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("Account")]
    public class AccountController : Saturn72ApiControllerBase
    {
        #region Fields

        private readonly IUserRegistrationService _userRegistrationService;

        #endregion

        #region ctor

        public AccountController(IUserRegistrationService userRegistrationService)
        {
            _userRegistrationService = userRegistrationService;
        }

        #endregion

        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationApiModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ConvertModelStateErrorsToKeyValuePair());

            //TOD: get password format from settings
            var request = new UserRegistrationRequest(model.Username, model.Email, model.Password, PasswordFormat.Encrypted,
                GetSenderIpAddress());
            var response = await _userRegistrationService.RegisterAsync(request);

            if (response.Success)
                return Ok();

            return BadRequestResult(response.Errors);
        }
    }
}