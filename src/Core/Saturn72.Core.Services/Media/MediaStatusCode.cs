namespace Saturn72.Core.Services.Media
{
    public class MediaStatusCode
    {
        public static MediaStatusCode Blocked = new MediaStatusCode(0,
            "The file type is blocked by the system");

        public static MediaStatusCode NotSupported = new MediaStatusCode(50,
            "The file type is not supported");

        public static MediaStatusCode Corrupted = new MediaStatusCode(100, "The file is corrupted");
        public static MediaStatusCode Valid = new MediaStatusCode(1200, "The file was validated");
        public static MediaStatusCode Invalid = new MediaStatusCode(200, "The file is invalid");
        public static MediaStatusCode Uploaded = new MediaStatusCode(1400, "The file was uploaded");

        private MediaStatusCode(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }
    }
}