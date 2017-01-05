using System.Collections.Generic;

namespace Saturn72.Core.Domain.Logging
{
    public sealed class LogLevel
    {
        private static LogLevel[] _allSystemLogLevels;

        public static readonly LogLevel Debug = new LogLevel(10, "debug");
        public static readonly LogLevel Trace = new LogLevel(20, "trace");
        public static readonly LogLevel Information = new LogLevel(30, "information");
        public static readonly LogLevel Warning = new LogLevel(40, "warning");
        public static readonly LogLevel Error = new LogLevel(50, "error");
        public static readonly LogLevel Fatal = new LogLevel(60, "fatal");

        public static readonly IEnumerable<LogLevel> AllSystemLogLevels =
            new[]
            {Debug, Information, Warning, Error, Fatal, Trace};

        private LogLevel(int code, string name)
        {
            Code = code;
            Name = name;
        }

        public int Code { get; }
        public string Name { get; }
    }
}