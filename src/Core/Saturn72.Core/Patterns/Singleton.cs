#region

using System;
using System.Collections.Generic;

#endregion

namespace Saturn72.Core.Patterns
{
    public abstract class Singleton<TService> : Singleton
    {
        private static TService _instance;

        /// <summary>
        ///     The singleton instance for the specified utilType TService. Only one instance (at the time) of this object for
        ///     each utilType of TService.
        /// </summary>
        public static TService Instance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                AllSingletons[typeof (TService)] = value;
            }
        }
    }

    /// <summary>
    ///     Provides access to all "singletons" stored by <see cref="Singleton{TService}" />.
    /// </summary>
    public class Singleton
    {
        private static readonly IDictionary<Type, object> _allSingletons;

        static Singleton()
        {
            _allSingletons = new Dictionary<Type, object>();
        }

        /// <summary>Dictionary of utilType to singleton instances.</summary>
        public static IDictionary<Type, object> AllSingletons
        {
            get { return _allSingletons; }
        }
    }
}