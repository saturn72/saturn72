#region

using System;

#endregion

namespace Saturn72.Core.Extensibility
{
    /// <summary>
    ///     Represents system module
    /// </summary>
    public class ModuleInstance
    {
        private IModule _module;

        public ModuleInstance(string type, bool active, int startupOrder, int stopOrder)

        {
            Type = type;
            Active = active;
            StartupOrder = startupOrder;
            StopOrder = stopOrder;
        }

        public int StopOrder { get; private set; }

        public int StartupOrder { get; private set; }

        /// <summary>
        ///     Gets module type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        ///     Gets value indicating if the module is active
        /// </summary>
        public bool Active { get; private set; }

        /// <summary>
        ///     Gets the instance of the module.
        ///     <remarks>The property is initialized after module has been loaded.</remarks>
        /// </summary>
        public IModule Module
        {
            get { return _module; }
        }

        public void SetModule(IModule module)
        {
            if (Module != null)
                throw new InvalidOperationException("Module can be initilzliaed only once.");

            _module = module;
        }
    }
}