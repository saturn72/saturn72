#region

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Web.Http;
using Owin;
using Saturn72.Core.Configuration.Maps;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Services.Authentication;
using Saturn72.Module.Owin.Adapters;
using Thinktecture.IdentityModel.Owin;

#endregion

namespace Saturn72.Module.Identity.ThinkTecture
{
    public class IdentityMiddlewareConfigurar : IOwinConfigurar
    {
        public int ConfigurationOrder
        {
            get { return 10; }
        }

        public void Configure(IAppBuilder app, HttpConfiguration httpConfig,
            IDictionary<string, IConfigMap> configurations)
        {
            var authenticationService = AppEngine.Current.Resolve<IAuthenticationService>();

            var authOpts = new BasicAuthenticationOptions("basic-authentication", async (username, pwd) =>
            {
                if (authenticationService.AuthenticateUserByUsernameAndPassword(username, pwd))
                {
                    return new[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }.AsEnumerable();
                }
                return null;
            });
            app.UseBasicAuthentication(authOpts);
        }
    }
}