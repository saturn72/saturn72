namespace Saturn72.Common.UI
{
    public abstract class StaticHtmlContentBase : IHtmlContent
    {
        private string _htmlKey;

        public string HtmlKey
        {
            get { return _htmlKey ?? (_htmlKey = GetType().FullName.Replace(".", "-")); }
        }

        public abstract string Html { get; }
    }
}