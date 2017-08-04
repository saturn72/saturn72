using System;
using System.Threading.Tasks;
using Saturn72.Core.Services.Datastructure;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Impl.Datastructure
{
    public class DatastructureConversionService:IDatastructureConversionService
    {
        public Task<object> Convert(DatastructureType sourceDatastructure, DatastructureType destinationDatastructure, byte[] bytes)
        {
            Guard.MustFollow(sourceDatastructure!=destinationDatastructure, () => throw new ArgumentException("Source and destination DatastructureTypes cannot be equal"));

            Guard.NotEmpty(bytes);
            Guard.NotNull(bytes);

            var convertor = GetConvertor(sourceDatastructure, destinationDatastructure);

            return Task.FromResult(convertor.Convert(bytes));
        }

        protected virtual IDatasourceConvertor GetConvertor(DatastructureType sourceDatastructure, DatastructureType destinationDatastructure)
        {
            throw new NotImplementedException();
        }
    }
}
