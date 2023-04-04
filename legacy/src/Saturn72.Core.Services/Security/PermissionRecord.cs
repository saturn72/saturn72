namespace Saturn72.Core.Services.Security
{
    public class PermissionRecord
    {
        protected PermissionRecord(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
