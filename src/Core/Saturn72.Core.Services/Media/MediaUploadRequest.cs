using System;
using System.IO;

namespace Saturn72.Core.Services.Media
{
    public class MediaUploadRequest
    {
        private string _extension;
        public Func<byte[]> Bytes { get; set; }
        public string FileName { get; set; }
        public string Extension => _extension ?? (_extension = Path.GetExtension(FileName).Replace(".", string.Empty));
    }
}