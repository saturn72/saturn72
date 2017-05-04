using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saturn72.Core.Services.File
{
    public interface IFileUploadManager
    {
        bool IsSupportedExtension(string extension);
        Task<IEnumerable<FileUploadResponse>> UploadAsync(IEnumerable<FileUploadRequest> requests);
    }
}