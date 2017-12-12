namespace Saturn72.Core.Services.File
{
    public class FileStatusCode
    {
        public static FileStatusCode Blocked = new FileStatusCode(0,
            "The file type is blocked by the system");

        public static FileStatusCode Unsupported = new FileStatusCode(50,
            "The file type is not supported");

        public static FileStatusCode Corrupted = new FileStatusCode(100, "The file is corrupted");
        public static FileStatusCode Valid = new FileStatusCode(1200, "The file was validated");
        public static FileStatusCode Invalid = new FileStatusCode(200, "The file is invalid");
        public static FileStatusCode Uploaded = new FileStatusCode(1400, "The file was uploaded");
        public static FileStatusCode FailedToUpload = new FileStatusCode(1600, "The file faild to upload");
        public static FileStatusCode UnexpectedError = new FileStatusCode(1800, "Unexpected error occured");
        public static FileStatusCode EmptyFile = new FileStatusCode(2000, "The file is empty");
        private FileStatusCode(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }
    }
}