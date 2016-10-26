#region

using System.IO;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.UI
{
    public abstract class StaticFileHtmlContentBase : StaticHtmlContentBase
    {
        private string _html;
        protected abstract string HtmlRelativePath { get; }

        public override string Html
        {
            get { return _html ?? (_html = RenderHtml()); }
        }

        protected virtual string RenderHtml()
        {
            var htmlFile = FileSystemUtil.RelativePathToAbsolutePath(HtmlRelativePath);
            using (var reader = File.OpenText(htmlFile))
            {
                return reader.ReadToEnd();
            }
        }
    }
}