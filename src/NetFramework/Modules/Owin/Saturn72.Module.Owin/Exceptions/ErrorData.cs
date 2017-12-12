#region

using System;

#endregion

namespace Saturn72.Module.Owin.Exceptions
{
    public class ErrorData
    {
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
        public Uri RequestUri { get; set; }
        public Guid ErrorId { get; set; }
    }
}