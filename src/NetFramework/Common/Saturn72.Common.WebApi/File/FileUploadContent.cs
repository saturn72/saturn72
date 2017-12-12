using System.Collections.Generic;
using Saturn72.Common.WebApi.Models;
using Saturn72.Core.Services.File;

namespace Saturn72.Common.WebApi.File
{
    public class FileUploadContent<TApiModel> where TApiModel : ApiModelBase, new()
    {
        public TApiModel Content { get; internal set; }
        public IEnumerable<FileUploadRequest> FileUploadRequests { get; internal set; }
    }
}