namespace Saturn72.Core
{
    internal class CacheKeys
    {
        internal const int DefaultCacheTime = 06 * 60 * 24;

        #region ComponentModel

        internal const int ConverterCacheTime = DefaultCacheTime; 
        internal const string ConverterByType = "converter-type:{0}";

        #endregion ComponentModel

    }
}
