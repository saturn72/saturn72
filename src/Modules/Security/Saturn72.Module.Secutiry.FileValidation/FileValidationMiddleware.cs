#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saturn72.Core;

#endregion

namespace Saturn72.Module.Secutiry.FileValidation
{
    #region

    using AppFunc = Func<IDictionary<string, object>, Task>;

    #endregion

    public class FileValidationMiddleware
    {
        private readonly AppFunc _next;

        public FileValidationMiddleware(AppFunc next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            //TODO: 
            //1.parse the environemnt dictionay 
            //2. if containis attachtment ==> validate attachtments
            // block or continue according to content
            //3. else continue
            //if does not have content ==> continue
            //if have attachtments ==> go to validation


            DefaultOutput.WriteLine("In File Validation Middleware - Not implemeted yet");
            await _next.Invoke(environment);


            //throw new NotImplementedException();

            //DefaultOutput.WriteLine(GetRequestDetails(environment));
            //DefaultOutput.WriteLine("Start Request Processing ...");

            //try
            //{
            //    await _next.Invoke(environment);
            //    DefaultOutput.WriteLine(GetResponseDetails(environment));
            //}
            //catch (Exception ex)
            //{
            //    //TODO: add log here
            //    DefaultOutput.WriteLine(ex.Message);
            //    DefaultOutput.WriteLine(ex);
            //}
        }

        //private string GetResponseDetails(IDictionary<string, object> environment)
        //{
        //    const string responseDetailsFormat = "Request ended: {0} with HTTP status {1}";
        //    return responseDetailsFormat.AsFormat(GetUri(environment), environment["owin.ResponseStatusCode"]);
        //}

        //private string GetRequestDetails(IDictionary<string, object> environment)
        //{
        //    const string requestDetailsFormat = "Request started: \"{0}\" {1}";
        //    var uri = GetUri(environment);
        //    return requestDetailsFormat.AsFormat(environment["owin.RequestMethod"], uri);
        //}

        //private string GetUri(IDictionary<string, object> environment)
        //{
        //    const string requestUriFormat = "{0}://{1}:{2}{3}";

        //    return (bool) environment["server.IsLocal"]
        //        ? requestUriFormat.AsFormat(
        //            environment["owin.RequestScheme"],
        //            environment["server.LocalIpAddress"],
        //            environment["server.LocalPort"],
        //            environment["owin.RequestPath"])
        //        : requestUriFormat.AsFormat(
        //            environment["owin.RequestScheme"],
        //            environment["server.RemoteIpAddress"],
        //            environment["server.RemotePort"],
        //            environment["owin.RequestPath"]);
        //}
    }
}