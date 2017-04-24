using System;

namespace Saturn72.Core.Domain.FileUpload
{
    public class MediaModel:DomainModelBase
    {
        public string FilePath { get; set; }
        public byte[] Bytes { get; set; }
        public Guid Guid { get; set; }
    }
}
