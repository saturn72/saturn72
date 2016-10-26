#region

using System;

#endregion

namespace Saturn72.Common.App
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AppVersionAttribute : Attribute
    {
        private readonly string _appVersionKey;

        public AppVersionAttribute(string appVersionKey)
        {
            _appVersionKey = appVersionKey;
        }

        public string AppVersionKey
        {
            get { return _appVersionKey; }
        }
    }
}