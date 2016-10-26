#region

using System;
using System.Text;

#endregion

namespace Saturn72.Extensions
{
    public static class ExceptionExtensions
    {
        private const string AsStringFormat = "{0}\n{1}";

        public static string AsString<TException>(this TException ex) where TException:Exception
        {
            if (ex == null)
                throw new NullReferenceException();

            var sb = new StringBuilder();
            var e = ex as Exception;
            do
            {
                sb.AppendLine(string.Format(AsStringFormat, e.Message, e));
                e = e.InnerException;
            } while (e != null);

            sb.Append(ex.StackTrace);
            return sb.ToString();
        }
    }
}