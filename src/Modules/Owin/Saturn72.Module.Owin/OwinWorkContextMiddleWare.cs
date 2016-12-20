using System;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Saturn72.Core;
using Saturn72.Core.Services;
using Saturn72.Extensions;

namespace Saturn72.Module.Owin
{
    public class OwinWorkContextMiddleWare<TUserId> : OwinMiddleware
    {
        private static TypeConverter _converter;

        private IWorkContext<TUserId> _workContext;

        public OwinWorkContextMiddleWare(OwinMiddleware next)
            : base(next)
        {
        }

        public void Initialized()
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

        public IWorkContext<TUserId> CurrentWorkContext => _workContext;
        protected void BuildWorkContext(IOwinContext context)
        {
            var identity = context.Request.User.Identity as ClaimsIdentity;
            if (identity.IsNull())
                return;
            var userId = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userId.IsNull())
                return;
            _workContext = new OwinWorkContext<TUserId>
            {
                CurrentUserId = (TUserId) _converter.ConvertFrom(userId.Value)
            };
        }
    }
}