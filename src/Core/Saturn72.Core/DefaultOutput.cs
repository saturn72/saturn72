#region

using System;
using System.IO;

#endregion

namespace Saturn72.Core
{
    public class DefaultOutput
    {
        private const string OutputPrefix = ">>>>>>";

        private static TextWriter OutputStream =>  Console.Out;

        public static void WriteLine(string format, params object[] args)
        {
            OutputStream.WriteLine(OutputPrefix + string.Format(format, args));
        }

        public static void WriteLine(object value)
        {
            WriteLine(value.ToString());
        }
    }
}