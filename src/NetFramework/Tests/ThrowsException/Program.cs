#region

using System;
using System.Threading;

#endregion

namespace ThrowsException
{
    class Program
    {
        static void Main()
        {
            Thread.Sleep(500);
            throw new Exception("This is expected exception that is thrown from executable");
        }
    }
}