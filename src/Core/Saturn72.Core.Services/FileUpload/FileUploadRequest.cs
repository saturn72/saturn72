using System;
using System.IO;

namespace Saturn72.Core.Services.FileUpload
{
    public class FileUploadRequest
    {
        private string _extension;
        public Func<byte[]> Bytes { get; set; }
        public string FilePath { get; set; }

        public string Extension => _extension ?? (_extension = Path.GetExtension(FilePath).Replace(".", string.Empty));
    }
}