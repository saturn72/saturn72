using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Saturn72.Common.WebApi.Models;
using Saturn72.Common.WebApi.MultistreamProviders;
using Saturn72.Common.WebApi.Utils;
using Saturn72.Core.Services.File;
using Saturn72.Extensions;

namespace Saturn72.Common.WebApi.File
{
    public class FileUploadModelBinder<TApiModel> : IModelBinder where TApiModel : ApiModelBase, new()
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var request = actionContext.Request;

            if (!request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotAcceptable,
                    "This request does not contain multipart data"));

            var model = new FileUploadContent<TApiModel>();
            TApiModel content = null;
            var attachtments = new List<FileUploadRequest>();

            //In case of files container
            var streamProvider = new InMemoryMultipartFormDataStreamProvider();
            request.Content.ReadAsMultipartAsync(streamProvider).Wait();

            //fot formdata: var _formData = streamProvider.FormData;

            var allHttpContentTypes = HttpContentType.AllHttpContentTypes;

            foreach (var httpContent in streamProvider.Contents)
            {
                var ct = allHttpContentTypes.FirstOrDefault(c => c.Match(httpContent));

                if (ct == HttpContentType.Model)
                {
                    var stream = httpContent.ReadAsStreamAsync().Result;
                    model.Content = JsonUtil.Deserialize<TApiModel>(stream);
                    continue;
                }

                if (ct == HttpContentType.File)
                {
                    var getBytesTask = httpContent.ReadAsStreamAsync().Result.ToByteArray();
                    var fileName = httpContent.GetContentDispositionFileName();
                    attachtments.Add(new FileUploadRequest
                    {
                        Bytes = getBytesTask,
                        FileName = fileName
                    });
                }
            }
            model.FileUploadRequests = attachtments;
            bindingContext.Model = model;
            return true;
        }
    }
}