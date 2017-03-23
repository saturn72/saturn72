namespace Saturn72.Core.Services.Impl
{
    public sealed class SystemSharedCacheKeys
    {
        public const string UserPatternCacheKey = "saturn72.user";
        public const string UserbyIdCacheKey = UserPatternCacheKey + ".id-{0}";
        public const string UserRolesUserCacheKey = UserbyIdCacheKey + ".userroles";
    }
}