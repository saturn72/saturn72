#region

using System;
using System.Runtime.Serialization;

#endregion

namespace Saturn72.Core.Exceptions
{
    [Serializable]
    public class Saturn72Exception : Exception
    {
        public Saturn72Exception()
        {
        }

        public Saturn72Exception(string message)
            : base(message)
        {
        }

        public Saturn72Exception(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        protected Saturn72Exception(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        public Saturn72Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}