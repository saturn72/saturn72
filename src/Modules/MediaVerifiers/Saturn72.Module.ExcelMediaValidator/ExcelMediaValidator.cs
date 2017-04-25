using System.Collections.Generic;
using Saturn72.Core.Services.Impl.Media;
using Saturn72.Core.Services.Media;

namespace Saturn72.Module.ExcelMediaValidator
{
    public class ExcelMediaValidator:IMediaValidator
    {
        public IEnumerable<string> SupportedExtensions { get; } = new []{"xls", "xlsx"};

        public MediaStatusCode Validate(byte[] bytes, string extension)
        {
            throw new System.NotImplementedException();
        }
    }
}
