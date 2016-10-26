#region

using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Saturn72.Core.Domain.Clients;
using Saturn72.Core.Services.Authentication;
using Saturn72.Core.Services.Impl.User;
using Saturn72.Core.Services.Security;
using Saturn72.Core.Services.User;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.Owin.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        #region Constst

        private const string UsernameKey = "userName";

        #endregion

        #region Fields

        private readonly IClientAppService _clientAppService;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;

        #endregion

        #region Ctor

        public SimpleAuthorizationServerProvider(IClientAppService clientAppService,
            IEncryptionService encryptionService,
            IUserRegistrationService userRegistrationService, IUserService userService, UserSettings userSettings)
        {
            _clientAppService = clientAppService;
            _encryptionService = encryptionService;
            _userRegistrationService = userRegistrationService;
            _userService = userService;
            _userSettings = userSettings;
        }

        #endregion

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            //legal request + client ID exists
            var authenticated = (context.TryGetBasicCredentials(out clientId, out clientSecret)
                                 || context.TryGetFormCredentials(out clientId, out clientSecret))
                                && context.ClientId.NotNull();

            var client = _clientAppService.GetClientByClientId(context.ClientId);
            //client exist by UsernameKey/password and nativeapp woth secret or not native app
            authenticated = authenticated && client.NotNull() && NativeAppCriteria(client, clientSecret);

            if (!authenticated)
            {
                RejectAndAddClientIdError(context);
                return Task.FromResult<object>(null);
            }

            //enable cors
            context.OwinContext.Set(SecurityKeys.ClientAllowedOrigin, client.AllowedOrigin);
            context.OwinContext.Set(SecurityKeys.ClientRefreshTokenLifeTime, client.RefreshTokenLifeTime.ToString());

            context.Validated();

            return Task.FromResult<object>(new {message = "Client was authenticated"});
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>(SecurityKeys.ClientAllowedOrigin);
            //if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {allowedOrigin});

            if (!_userRegistrationService.ValidateUserByUsernameAndPassword(context.UserName, context.Password))
            {
                AddErrorToContext(context, "invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var user = _userSettings.ValidateByEmail
                ? _userService.GetUserByEmail(context.UserName)
                : _userService.GetUserByUsername(context.UserName);
            Guard.NotNull(user);

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
            identity.AddClaim(new Claim("sub", context.UserName));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    SecurityKeys.ClientId, context.ClientId ?? string.Empty
                },
                {
                    UsernameKey, context.UserName
                }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary[SecurityKeys.ClientId];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                AddErrorToContext(context, "invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.Where(c => c.Type == "newClaim").FirstOrDefault();
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        #region Utilities

        private bool NativeAppCriteria(ClientAppDomainModel client, string clientSecret)
        {
            return client.ApplicationType == ApplicationType.NativeApp
                ? clientSecret.HasValue() && _encryptionService.DecryptText(client.Secret) == clientSecret
                : true;
        }

        private static void RejectAndAddClientIdError(OAuthValidateClientAuthenticationContext context)
        {
            context.Rejected();
            AddErrorToContext(context, "invalid_client", "Illegal client");
        }

        private static void AddErrorToContext<TOptions>(BaseValidatingContext<TOptions> context, string error,
            string errorDescription)
        {
            context.SetError(error,
                JsonConvert.SerializeObject(
                    new {result = false, message = errorDescription}));
        }

        #endregion
    }
}