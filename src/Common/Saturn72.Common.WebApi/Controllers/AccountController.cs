#region

using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Saturn72.Core.Domain.Users;
using Saturn72.Core.Services.User;
using Saturn72.Common.WebApi;
using Saturn72.Common.WebApi.Models.Account;

#endregion

namespace Saturn72.Common.WebApi.Controllers
{
    [AllowAnonymous]
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

        public async Task<IHttpActionResult> Register(UserRegistrationApiModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //TOD: get password format from settings
            var request = new UserRegistrationRequest(model.Username, model.Password, PasswordFormat.Encrypted, GetSenderIpAddress());
            var response = await _userRegistrationService.RegisterAsync(request);

            if (response.Success)
                return Ok();

            return BadRequest(response.Errors.Aggregate((x, y) => x + "\n" + y));
        }
    }
}