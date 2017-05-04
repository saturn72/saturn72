using System;

namespace Saturn72.Core.Domain.FileUpload
{
    public class FileUploadRecordModel:DomainModelBase
    {
        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
        public Guid UploadId { get; set; }
        public virtual FileUploadSessionModel UploadSession { get; set; }
        public long UploadSessionId { get; set; }
    }
}
