namespace Saturn72.Core.Services.FileUpload
{
    public abstract class FileUploadManagerBase : IFileUploadManager
    {
        public abstract bool IsSupported(FileUploadRequest fileUploadRequest);
        public void Upload(FileUploadRequest fileUploadRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}