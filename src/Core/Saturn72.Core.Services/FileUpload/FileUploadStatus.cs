namespace Saturn72.Core.Services.FileUpload
{
    public class FileUploadStatus
    {
        private FileUploadStatus(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }
        public string Message { get; }

        #region statuses
        public static FileUploadStatus Uploaded = new FileUploadStatus(1200, "Uploaded");
        public static FileUploadStatus Invalid = new FileUploadStatus(1400, "Invalid");

        #endregion
    }
}