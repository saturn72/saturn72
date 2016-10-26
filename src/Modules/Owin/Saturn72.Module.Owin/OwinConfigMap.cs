#region

using System.Collections.Generic;
using System.Xml;
using Saturn72.Core.Configuration;
using Saturn72.Core.Configuration.Maps;

#endregion

namespace Saturn72.Module.Owin
{
    public class OwinConfigMap : ConfigSectionConfigMapBase<OwinConfig>
    {
        public OwinConfigMap(string name, string configFilePath) : base(name, configFilePath)
        {
        }
    }

    public class OwinConfig : ConfigurationSectionHandlerBase
    {
        public string ServerUri { get; private set; }
        public bool UseExtrnalCookies { get; private set; }
        public bool UseOAuth { get; private set; }
        public bool EnableVersioning { get; private set; }
        public IEnumerable<OAuthProvider> OAuthProviders { get; private set; }
        public bool UseCors { get; private set; }

        public override object Create(object parent, object configContext, XmlNode section)
        {
            var config = section.SelectSingleNode("configMap");
            ServerUri = config.SelectSingleNode("serverUri").InnerText;
            UseExtrnalCookies = GetNodeValueOrDefault(config.SelectSingleNode("useExternalCookies"), true);
            UseOAuth = GetNodeValueOrDefault(config.SelectSingleNode("useOAuth"), true);
            EnableVersioning = GetNodeValueOrDefault(config.SelectSingleNode("enableVersioning"), false);
            UseCors = GetNodeValueOrDefault(config.SelectSingleNode("useCors"), true);

            var providersNode = config.SelectSingleNode("oAuthProviders");
            if (providersNode.HasChildNodes)
                GetOAuthProviders(providersNode.ChildNodes);

            return this;
        }

        private void GetOAuthProviders(XmlNodeList providersNodeList)
        {
            var tmp = new List<OAuthProvider>();
            var enm = providersNodeList.GetEnumerator();

            while (enm.MoveNext())
            {
                var xn = (enm.Current as XmlNode);

                tmp.Add(new OAuthProvider
                {
                    Name = xn.Attributes["name"].Value,
                    AppSecret = xn.Attributes["appSecret"].Value,
                    AppId = xn.Attributes["appId"].Value
                });
            }

            OAuthProviders = tmp;
        }
    }

    public class OAuthProvider
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Name { get; set; }
    }
}