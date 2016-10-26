namespace Saturn72.Core.Services.Security
{
    public interface IFileValidationManager
    {
        FileValidationResult ValidateFile(FileValidationRequest request);
    }
}