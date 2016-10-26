namespace Saturn72.Core.Services.Security
{
    public class FileValidationResultCode
    {

        private FileValidationResultCode(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }

        public string Message { get; }


        public static FileValidationResultCode Blocked = new FileValidationResultCode(0,
            "The file type is blocked by the system");

        public static FileValidationResultCode NotSupported = new FileValidationResultCode(50,
            "The file type is not supported");

        public static FileValidationResultCode Corrupted = new FileValidationResultCode(100, "The file is corrupted");
        public static FileValidationResultCode Validated = new FileValidationResultCode(1200, "The file was validated");
    }
}