#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Saturn72.Common.WebApi.Models;
using Saturn72.Common.WebApi.MultistreamProviders;
using Saturn72.Common.WebApi.Utils;
using Saturn72.Core;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi
{
    //[Authorize]
    public abstract class Saturn72ApiAuthorizedControllerBase : Saturn72ApiControllerBase
    {
    }
}