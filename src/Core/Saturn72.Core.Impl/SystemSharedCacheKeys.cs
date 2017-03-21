namespace Saturn72.Core.Services.Impl
{
    public sealed class SystemSharedCacheKeys
    {
        public const string UserPatternCacheKey = "saturn72.user";
        public const string UserByIdCacheKey = UserPatternCacheKey + ".id-{0}";
        public const string UserRolesUserCacheKey = UserByIdCacheKey + ".userroles";
    }
}