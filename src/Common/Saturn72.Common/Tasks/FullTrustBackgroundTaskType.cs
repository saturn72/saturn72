#region

using System;
using System.IO;
using System.Reflection;
using Saturn72.Common.UI;
using Saturn72.Core;
using Saturn72.Core.Tasks;

#endregion

namespace Saturn72.Common.Tasks
{
    public class FullTrustBackgroundTaskType : StaticFileHtmlContentBase, IBackgroundTaskType
    {
        private Type _executorType;

        protected override string HtmlRelativePath
        {
            get { return GetCrateViewPath(); }
        }

        public string UniqueIdentifier
        {
            get { return CommonHelper.GetCompatibleTypeName(GetType()); }
        }

        public string DisplayName
        {
            get { return "Full Trust Task"; }
        }

        public string Description
        {
            get { return "Runs the task in full trust mode."; }
        }

        public Type ExecutorType
        {
            get { return _executorType ?? (_executorType = typeof (FullTrustBackgroundTask)); }
        }

        private string GetCrateViewPath()
        {
            var asmLocation = Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(asmLocation), "views", GetType().Name, "create.html");
        }

        public long Id { get; set; }
    }
}