
using System;
using System.IO;

namespace Saturn72.Common
{
    public sealed class DefaultOutput
    {
        private const string OutputPrefix = ">>>>>>";

        private static TextWriter _outputStream;

        private static TextWriter OutputStream
        {
            get { return _outputStream ?? (_outputStream = Console.Out); }
        }

        public static void WriteLine(string line)
        {
            OutputStream.WriteLine(OutputPrefix + line);
        }

        public static void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }
    }
}
