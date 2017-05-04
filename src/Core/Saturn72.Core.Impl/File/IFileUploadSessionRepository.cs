using Saturn72.Core.Domain.FileUpload;

namespace Saturn72.Core.Services.Impl.File
{
    public interface IFileUploadSessionRepository
    {
        void Create(FileUploadSessionModel session);
    }
}