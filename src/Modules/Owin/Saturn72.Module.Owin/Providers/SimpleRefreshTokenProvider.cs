#region

using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Saturn72.Core;
using Saturn72.Core.Domain.Security;
using Saturn72.Core.Services.Authentication;
using Saturn72.Core.Services.Security;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.Owin.Providers
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        #region ctor

        public SimpleRefreshTokenProvider(IEncryptionService encryptionService,
            IAuthenticationService authenticationService)
        {
            _encryptionService = encryptionService;
            _authenticationService = authenticationService;
        }

        #endregion

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientId = context.Ticket.Properties.Dictionary[SecurityKeys.ClientId];
            if (!clientId.HasValue())
                return;

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime =
                CommonHelper.ToInt(context.OwinContext.Get<string>(SecurityKeys.ClientRefreshTokenLifeTime));
            if (refreshTokenLifeTime.IsDefault())
                refreshTokenLifeTime = 30;


            var token = new RefreshTokenDomainModel
            {
                Hash = _encryptionService.EncryptText(refreshTokenId),
                ClientId = clientId,
                Subject = context.Ticket.Identity.Name,
                IssuedOnUtc = DateTime.UtcNow,
                ExpiresOnUtc = DateTime.UtcNow.AddMinutes(refreshTokenLifeTime)
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedOnUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresOnUtc;

            token.ProtectedTicket = context.SerializeTicket();

            await _authenticationService.AddRefreshTokenAsync(token);
            context.SetToken(refreshTokenId);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();

            //var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            //string hashedTokenId = Helper.GetHash(context.Token);

            //using (AuthRepository _repo = new AuthRepository())
            //{
            //    var refreshToken = await _repo.FindRefreshToken(hashedTokenId);

            //    if (refreshToken != null)
            //    {
            //        //Get protectedTicket from refreshToken class
            //        context.DeserializeTicket(refreshToken.ProtectedTicket);
            //        var result = await _repo.RemoveRefreshToken(hashedTokenId);
            //    }
            //}
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IEncryptionService _encryptionService;

        #endregion
    }
}