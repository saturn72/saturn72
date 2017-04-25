using System;

namespace Saturn72.Core.Domain.FileUpload
{
    public class MediaModel:DomainModelBase
    {
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
        public Guid UploadSessionId { get; set; }
    }
}
