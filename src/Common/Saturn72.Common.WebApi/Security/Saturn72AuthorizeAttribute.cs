using System;

namespace Saturn72.Common.WebApi.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class Saturn72AuthorizeAttribute : Attribute
    //public class Saturn72Authorized : Authorize
    {
    }
}