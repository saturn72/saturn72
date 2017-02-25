namespace Saturn72.Core.Services.Validation
{
    public sealed class SystemErrorCode
    {
        public SystemErrorCode(string code, string message, string category)
        {
            Code = code;
            Message = message;
            Category = category;
        }

        public string Code { get; }

        public string Message { get; }

        public string Category { get; }
    }
}