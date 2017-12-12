using System;
using System.Collections.Generic;

namespace Saturn72.Core.Domain.FileUpload
{
    public class FileUploadSessionModel : DomainModelBase
    {
        private IEnumerable<FileUploadRecordModel> _fileUploadRecords;
        public Guid SessionId { get; set; }

        public virtual IEnumerable<FileUploadRecordModel> FileUploadRecords => _fileUploadRecords ?? (_fileUploadRecords = new List<FileUploadRecordModel>());
    }
}