using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saturn72.Core.Services.Media
{
    public interface IMediaUploadManager
    {
        bool IsSupportedExtension(string extension);
        Task<IEnumerable<MediaUploadResponse>> UploadAsync(IEnumerable<MediaUploadRequest> requests);
    }
}