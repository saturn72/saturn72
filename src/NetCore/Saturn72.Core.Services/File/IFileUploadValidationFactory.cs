
namespace Saturn72.Core.Services.File
{
    public interface IFileUploadValidationFactory
    {
        bool IsSupportedExtension(string fileExtension);
        FileStatusCode Validate(string extension, byte[] bytes);
    }
}