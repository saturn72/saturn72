using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Saturn72.Core.Infrastructure;

namespace Saturn72.Core.Extensibility
{
    public class PluginDescriptor
    {
        #region Fields

        private Type _type;
        private Assembly _referencedAssembly;

        #endregion

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }
        public string SystemName { get; set; }
        public string Version { get; set; }
        public PluginState State { get; set; }
        public string Group { get; set; }
        public Type PluginType
        {
            get { return _type ?? (_type = CommonHelper.GetTypeFromAppDomain(TypeFullName)); }
        }

        public Assembly ReferencedAssembly { get
        {
            return _referencedAssembly ?? (_referencedAssembly = PluginType.Assembly);
        } }
        public string TypeFullName { get; set; }
        public TPlugin Instance<TPlugin>() where TPlugin : class, IPlugin
        {
            return CommonHelper.CreateInstance<TPlugin>(TypeFullName);
        }

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }
    }
}