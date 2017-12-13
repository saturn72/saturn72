using System;
using System.IO;
using Saturn72.Extensions;

namespace Saturn72.Core.Domain.FileUpload
{
    public class FileUploadRecordModel : DomainModelBase
    {
        private string _extension;

        public string FileName { get; set; }
        public byte[] Bytes { get; set; }
        public Guid UploadId { get; set; }
        public virtual FileUploadSessionModel UploadSession { get; set; }
        public long UploadSessionId { get; set; }

        public string Extension
        {
            get
            {
                if (FileName.HasValue())
                    return _extension ?? (_extension = Path.GetExtension(FileName).Replace(".", string.Empty));
                return null;
            }
        }
    }
}