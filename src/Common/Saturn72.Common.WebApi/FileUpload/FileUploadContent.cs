using System.Collections.Generic;
using Saturn72.Common.WebApi.Models;
using Saturn72.Core.Services.Media;

namespace Saturn72.Common.WebApi.FileUpload
{
    public class FileUploadContent<TApiModel> where TApiModel : ApiModelBase, new()
    {
        public TApiModel Content { get; internal set; }
        public IEnumerable<MediaUploadRequest> FileUploadRequests { get; internal set; }
    }
}