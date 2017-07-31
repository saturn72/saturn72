
namespace Saturn72.Core.Services.File
{
    public interface IFileUploadValidationFactory
    {
        bool IsSupportedExtension(string fileExtension);
        FileStatusCode Validate(FileUploadRequest fileUploadRequest);
    }
}