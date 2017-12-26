using System;
using System.Collections.Generic;
using Saturn72.Core.Audit;

namespace Saturn72.Core.Domain.FileUpload
{
    public class FileUploadSessionModel : DomainModelBase, ICreateAudit
    {
        private IEnumerable<FileUploadRecordModel> _fileUploadRecords;
        public Guid SessionId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public long CreatedByUserId { get; set; }

        public virtual IEnumerable<FileUploadRecordModel> FileUploadRecords
        {
            get => _fileUploadRecords ?? (_fileUploadRecords = new List<FileUploadRecordModel>());
            set => _fileUploadRecords = value;
        }
    }
}