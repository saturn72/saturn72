using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Saturn72.Core;
using Saturn72.Core.Infrastructure;
using Saturn72.Extensions;

namespace Saturn72.Module.Owin.Middlewares
{
    public class OwinWorkContextMiddleWare : OwinMiddleware
    {
        private readonly IWorkContext _workContext;

        public OwinWorkContextMiddleWare(IWorkContext workContext, OwinMiddleware next)
            : base(next)
        {
            _workContext = workContext;
        }

        public override async Task Invoke(IOwinContext context)
        {
            BuildWorkContext(context);
            await Next.Invoke(context);
        }

        protected void BuildWorkContext(IOwinContext context)
        {
            _workContext.CurrentUserIpAddress = context.Request.RemoteIpAddress;
            _workContext.ClientId = context.Request.Headers["client_id"];

            var identity = context.Request.User?.Identity as ClaimsIdentity;
            if (identity.NotNull())
                GetUserId(identity);
        }

        private void GetUserId(ClaimsIdentity identity)
        {
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim.IsNull())
                return;

            long userId;
            long.TryParse(userIdClaim.Value, out userId);
            _workContext.CurrentUserId = userId;
        }
    }
}