namespace Saturn72.Core.Domain.Logging
{
    public sealed class LogLevel
    {
        public static LogLevel Debug = new LogLevel(10, "debug");
        public static LogLevel Information = new LogLevel(20, "information");
        public static LogLevel Warning = new LogLevel(30, "warning");
        public static LogLevel Error = new LogLevel(40, "error");
        public static LogLevel Fatal = new LogLevel(50, "fatal");

        private LogLevel(int code, string name)
        {
            Code = code;
            Name = name;
        }

        public int Code { get; }
        public string Name { get; }
    }
}