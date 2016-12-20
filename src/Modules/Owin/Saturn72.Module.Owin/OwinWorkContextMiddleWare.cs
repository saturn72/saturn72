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

        public OwinWorkContextMiddleWare(OwinMiddleware next)
            : base(next)
        {

        }


        public IWorkContext<TUserId> CurrentWorkContext { get; private set; }

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
            var identity = context.Request.User?.Identity as ClaimsIdentity;
            if (identity.IsNull())
                return;
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim.IsNull())
                return;
            CurrentWorkContext = AppEngine.Current.Resolve<IWorkContext<TUserId>>();
            CurrentWorkContext.CurrentUserId = (TUserId) _converter.ConvertFrom(userIdClaim.Value);
        }
    }
}