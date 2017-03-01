using System.Web.Http;
using Owin;
using Saturn72.Module.Owin.Adapters;

namespace Saturn72.Module.Secutiry.FileValidation
{
    public class FileValidationOwinConfigurar : IOwinConfigurar
    {
        public bool InvokeBeforeOwinCommonMiddlewares => false;
        public int ConfigurationOrder => 10;

        public void Configure(IAppBuilder app, HttpConfiguration httpConfig)
        {
            app.Use(typeof(FileValidationMiddleware));
        }
    }
}