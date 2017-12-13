
namespace Saturn72.Core.Services.File
{
    public interface IFileHandlerFactory
    {
        bool IsSupportedExtension(string fileExtension);
        FileStatusCode Validate(string extension, byte[] bytes);
        byte[] Format(string sourceFormat, string destinationFormat, byte[] sourceBytes);
        byte[] Minify(byte[] bytes, string fileExtension);
    }
}