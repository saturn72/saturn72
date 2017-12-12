#region

using System;

#endregion

namespace Saturn72.Core.Services.App
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AppVersionAttribute : Attribute
    {
        public AppVersionAttribute(string appVersionKey)
        {
            AppVersionKey = appVersionKey;
        }

        public string AppVersionKey { get; }
    }
}