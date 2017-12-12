using System;
using System.IO;
using System.Reflection;

namespace Saturn72.Core.Extensibility
{
    public class PluginDescriptor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public PluginState State { get; set; }
        public string Group { get; set; }

        public Type PluginType => _type ?? (_type = CommonHelper.GetTypeFromAppDomain(TypeFullName));

        public Assembly ReferencedAssembly => _referencedAssembly ?? (_referencedAssembly = PluginType.Assembly);

        public string TypeFullName { get; set; }
        public FileInfo DescriptorFile { get; set; }

        public TPlugin Instance<TPlugin>() where TPlugin : class, IPlugin
        {
            return CommonHelper.CreateInstance<TPlugin>(TypeFullName);
        }

        public IPlugin Instance()
        {
            return Instance<IPlugin>();
        }

        #region Fields

        private Type _type;
        private Assembly _referencedAssembly;

        #endregion
    }
}