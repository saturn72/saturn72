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
        void Load();

        /// <summary>
        ///     starts the module
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops the module
        /// </summary>
        void Stop();
    }
}