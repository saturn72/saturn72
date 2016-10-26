#region

using System;

#endregion

namespace Saturn72.Core.Services.Security
{
    public class FileValidationRequest
    {
        public FileValidationRequest(FileContent fileContent)
        {
            FileContent = fileContent;
            CreatedOnUtc = DateTime.UtcNow;
        }

        public DateTime CreatedOnUtc { get; }
        public FileContent FileContent { get; }
    }
}