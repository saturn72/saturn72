#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Saturn72.Common.WebApi.Models;
using Saturn72.Common.WebApi.MultistreamProviders;
using Saturn72.Common.WebApi.Utils;
using Saturn72.Core;
using Saturn72.Core.Services.Security;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Common.WebApi
{
    public abstract class Saturn72ApiControllerBase : ApiController
    {
        protected NameValueCollection FormData { get; private set; }

        protected ClaimsIdentity Identity
        {
            get { return User.Identity as ClaimsIdentity; }
        }

        protected Task<IHttpActionResult> ValidateModelStateAndRunActionAsync(Action action)
        {
            return ValidateModelStateAndRunActionAsync(() => ModelState.IsValid, action);
        }

        protected Task<IHttpActionResult> ValidateModelStateAndRunActionAsync(Func<bool> validationFunc,
           Action action)
        {
            Exception exception = null;
            return Task.Run<IHttpActionResult>(() =>
            {
                if (!validationFunc())
                    return BadRequest(ConvertModelStateErrorsToKeyValuePair());

                try
                {
                    action();
                    return Ok();
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return BadRequest();
                }
                finally
                {
                    if (exception.NotNull())
                        throw exception;
                }
            });
        }

        protected Task<IHttpActionResult> ValidateModelStateAndRunActionAsync<TResult>(Func<TResult> func)
        {
            return ValidateModelStateAndRunActionAsync(() => ModelState.IsValid, func);
        }

        protected Task<IHttpActionResult> ValidateModelStateAndRunActionAsync<TResult>(Func<bool> validationFunc,
            Func<TResult> func)
        {
            Exception exception = null;
            return Task.Run<IHttpActionResult>(() =>
            {
                if (!validationFunc())
                    return BadRequest(ConvertModelStateErrorsToKeyValuePair());

                try
                {
                    return Ok(func());
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return BadRequest();
                }
                finally
                {
                    if (exception.NotNull())
                        throw exception;
                }
            });
        }

        private string ConvertModelStateErrorsToKeyValuePair()
        {
            var modelStateErrorsList = ModelState.Select(x =>
                string.Format("{0} : {1}", x.Key, string.Join("\n\t", x.Value.Errors.Select(e => e.ErrorMessage))))
                .ToArray();

            return string.Join("\n", modelStateErrorsList);
        }

        protected virtual string GetSenderIpAddress()
        {
            return CommonHelper.IsWebApp()
                ? HttpContext.Current.Request.UserHostAddress
                : Request.GetOwinContext().Request.RemoteIpAddress;
        }

        protected IEnumerable<Claim> GetClaims()
        {
            return Identity.Claims;
        }

        protected Claim GetIdClaim()
        {
            return Identity.FindFirst(ClaimTypes.NameIdentifier);
        }

        protected virtual async Task<TApiModel> ExtractDomainModelFromMultipartRequestAsync<TApiModel>
            (ICollection<FileContent> attachtments)
            where TApiModel : ApiModelBase, new()
        {
            return await ExtractDomainModelFromMultipartRequestAsync<TApiModel>(Request, attachtments);
        }

        protected virtual async Task<TApiModel> ExtractDomainModelFromMultipartRequestAsync<TApiModel>
            (HttpRequestMessage request, ICollection<FileContent> attachtments)
            where TApiModel : ApiModelBase, new()
        {
            TApiModel model = null;

            return await ExtractDomainModelFromMultipartRequestAsync(request, model, attachtments);
        }

        protected virtual async Task<TApiModel> ExtractDomainModelFromMultipartRequestAsync<TApiModel>
            (HttpRequestMessage request, TApiModel model, ICollection<FileContent> attachtments)
            where TApiModel : ApiModelBase, new()
        {
            Guard.NotNull(request);

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable,
                    "This request does not contain multipart data"));
            }

            var streamProvider = new InMemoryMultipartFormDataStreamProvider();
            await request.Content.ReadAsMultipartAsync(streamProvider);
            FormData = streamProvider.FormData;

            var allHttpContentTypes = HttpContentType.AllHttpContentTypes;

            foreach (var httpContent in streamProvider.Contents)
            {
                var ct = allHttpContentTypes.FirstOrDefault(c => c.Match(httpContent));
                var stream = await httpContent.ReadAsStreamAsync();

                if (ct == HttpContentType.Model)
                {
                    model = JsonUtil.Deserialize<TApiModel>(stream);
                    continue;
                }

                if (ct == HttpContentType.File)
                {
                    //select primary - file - else to collection
                    var fileName = httpContent.GetContentDispositionFileName();
                    attachtments.Add(new FileContent
                    {
                        Bytes = stream.ToByteArray(),
                        FilePath = fileName
                    });
                }
            }

            return model;
        }

        
    }
}