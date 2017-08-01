using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core.Domain.FileUpload;

namespace Saturn72.Core.Services.File
{
    public interface IFileUploadService
    {
        bool IsSupportedExtension(string extension);
        Task<IEnumerable<FileUploadResponse>> UploadAsync(IEnumerable<FileUploadRequest> requests);
        Task<IEnumerable<FileUploadRecordModel>> GetFileUploadRecordByUploadSessionIdAsync(long uploadSessionId);
    }
}