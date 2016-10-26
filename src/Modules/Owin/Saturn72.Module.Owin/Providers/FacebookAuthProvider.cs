#region

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;

#endregion

namespace Saturn72.Module.Owin.Providers
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }
    }
}