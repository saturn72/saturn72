using System.Net.Http;
using System.Text;
using System.Web.Http.ExceptionHandling;
using Saturn72.Core;

namespace Saturn72.Module.Owin.Exceptions
{
    public class ConsoleExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            DefaultOutput.WriteLine("Error has accured: " + RequestToString(context.Request));
            base.Log(context);
        }

        private string RequestToString(HttpRequestMessage request)
        {
            var message = new StringBuilder();
            if (request.Method != null)
                message.Append(request.Method);
            if (request.RequestUri != null)
                message.Append(" ").Append(request.RequestUri);
            return message.ToString();
        }
    }
}