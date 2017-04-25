using System;
using Saturn72.Core.Domain.FileUpload;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Media
{
    public class MediaUploadResponse
    {
        public MediaUploadResponse(MediaUploadRequest request, MediaStatusCode status, MediaModel media, string message)
        {
            Request = request;
            Status = status;
            Media = media;
            Message = message;
        }
        public MediaUploadRequest Request { get; }
        public MediaStatusCode Status { get; }
        public MediaModel Media { get; }
        public string Message { get; }
        public bool WasUploaded => Status == MediaStatusCode.Uploaded && Media.NotNull() && Media.UploadSessionId != Guid.Empty;
    }
}