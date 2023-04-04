#region Usings

using Saturn72.Core.Services.File;

#endregion

namespace Saturn72.Core.Services.Web
{
    public class MultipartRequestParsedObjects<T> where T : new()
    {
        public MultipartRequestParsedObjects()
        {
            Model = new T();
        }
        public FileUploadRequest FileUploadRequest { get; set; }
        public T Model { get; set; }
    }
}