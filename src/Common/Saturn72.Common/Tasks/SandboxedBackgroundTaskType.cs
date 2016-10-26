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
    public class SandboxedBackgroundTaskType : StaticFileHtmlContentBase, IBackgroundTaskType
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
            get { return "Sandboxed Task (Medium to low trust)"; }
        }

        public string Description
        {
            get { return "Runs the task in sandbox mode."; }
        }

        public Type ExecutorType
        {
            get { return _executorType ?? (_executorType = typeof (SandboxedBackgroundTask)); }
        }

        private string GetCrateViewPath()
        {
            var asmLocation = Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(asmLocation), "views", GetType().Name, "create.html");
        }

        public long Id { get; set; }
    }
}