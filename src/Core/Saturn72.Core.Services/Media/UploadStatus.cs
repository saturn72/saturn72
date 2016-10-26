namespace Saturn72.Core.Services.Media
{
    public class UploadStatus
    {
        private UploadStatus(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }
        public string Message { get; }

        #region statuses
        public static UploadStatus Uploaded = new UploadStatus(1200, "Uploaded");

        #endregion
    }
}