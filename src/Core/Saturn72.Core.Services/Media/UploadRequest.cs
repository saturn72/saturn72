using Saturn72.Core.Services.Security;

namespace Saturn72.Core.Services.Media
{
    public class UploadRequest
    {
        public UploadRequest(FileContent fileContentses)
        {
            this.FileContent = fileContentses;
        }

        public FileContent FileContent { get; }
    }
}