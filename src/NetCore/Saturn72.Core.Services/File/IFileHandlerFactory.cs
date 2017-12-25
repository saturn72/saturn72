
namespace Saturn72.Core.Services.File
{
    public interface IFileHandlerFactory
    {
        bool IsSupportedExtension(string fileExtension);
        FileStatusCode Validate(string extension, byte[] bytes);
        byte[] Minify(byte[] bytes, string fileExtension);
    }
}