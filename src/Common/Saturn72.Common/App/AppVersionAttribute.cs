#region

using System;

#endregion

namespace Saturn72.Common.App
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