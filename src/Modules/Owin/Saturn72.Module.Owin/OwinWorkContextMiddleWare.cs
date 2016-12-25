using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Saturn72.Core;
using Saturn72.Core.Infrastructure;
using Saturn72.Extensions;

namespace Saturn72.Module.Owin
{
    public class OwinWorkContextMiddleWare<TUserId> : OwinMiddleware
    {
        private static TypeConverter _converter;

        public OwinWorkContextMiddleWare(IWorkContext<TUserId> workContext, OwinMiddleware next)
            : base(next)
        {
            CurrentWorkContext = workContext;
        }

        public IWorkContext<TUserId> CurrentWorkContext { get; }

        public static void Initialized()
        {
            _converter = CommonHelper.GetCustomTypeConverter(typeof(TUserId));
            if (!_converter.CanConvertTo(typeof(TUserId)))
                throw new NotSupportedException("OwinWorkContextMiddleware cannot convert user Identity to typeof " +
                                                typeof(TUserId).FullName);
        }

        public override async Task Invoke(IOwinContext context)
        {
            BuildWorkContext(context);
            await Next.Invoke(context);
        }

        protected void BuildWorkContext(IOwinContext context)
        {
            CurrentWorkContext.CurrentUserIpAddress = context.Request.RemoteIpAddress;
            CurrentWorkContext.ClientId = context.Request.Headers["client_id"];

            var identity = context.Request.User?.Identity as ClaimsIdentity;
            if (identity.IsNull())
                return;

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim.NotNull())
                CurrentWorkContext.CurrentUserId = (TUserId) _converter.ConvertFrom(userIdClaim.Value);
        }
    }
}