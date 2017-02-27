namespace Saturn72.Common.App
{
    public sealed class AppVersionStatusType
    {
        public static readonly AppVersionStatusType Alpha = new AppVersionStatusType("Alpha",
            "4FC974B9-8F54-473E-9540-DA9D171484B3");

        public static readonly AppVersionStatusType Beta = new AppVersionStatusType("Beta",
            "5DB58464-8591-414B-B53C-39F5802455CB");

        public static readonly AppVersionStatusType ReleaseCandidate = new AppVersionStatusType("ReleaseCandidate",
            "C87BA419-69E1-4EA8-A774-6AFDC5C697D9");

        public static readonly AppVersionStatusType Stable = new AppVersionStatusType("Stable",
            "4FA85E17-7A91-41F4-8EF3-597270AF1750");

        private AppVersionStatusType(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public string Name { get; }

        public string Code { get; }
    }
}