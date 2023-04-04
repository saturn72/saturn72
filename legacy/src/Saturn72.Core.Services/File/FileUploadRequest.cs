using System.IO;

namespace Saturn72.Core.Services.File
{
    public class FileUploadRequest
    {
        private string _extension;
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }
        public string Extension => _extension ?? (_extension = Path.GetExtension(FileName).Replace(".", string.Empty));
    }
}