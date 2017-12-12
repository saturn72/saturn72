using System.Diagnostics;

namespace HelloWorld
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var consoleTraceListener = new ConsoleTraceListener();
            Trace.Listeners.Add(consoleTraceListener);
            var app = new HelloWorldApp();
            app.Start();
            Trace.Listeners.Remove(consoleTraceListener);
        }
    }
}