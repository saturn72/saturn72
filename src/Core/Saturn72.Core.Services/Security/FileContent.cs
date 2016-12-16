using System;

namespace Saturn72.Core.Services.Security
{
    public class FileContent
    {
        public Func<byte[]> Bytes { get; set; }
        public string FilePath { get; set; }
    }
}