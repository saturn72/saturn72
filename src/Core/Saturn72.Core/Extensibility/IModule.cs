#region

using System.Collections.Generic;
using Saturn72.Core.Configuration.Maps;

#endregion

namespace Saturn72.Core.Extensibility
{
    public interface IModule
    {
        /// <summary>
        ///     Loads the module
        /// </summary>
        /// <param name="configurations"></param>
        void Load(IDictionary<string, IConfigMap> configurations);

        /// <summary>
        ///     starts the module
        /// </summary>
        /// <param name="configuration"></param>
        void Start(IDictionary<string, IConfigMap> configuration);

        /// <summary>
        ///     Stops the module
        /// </summary>
        /// <param name="configurations"></param>
        void Stop(IDictionary<string, IConfigMap> configurations);
    }
}