using Saturn72.Common.WebApi.Models;
using Saturn72.Mappers;

namespace Mimas.WebApi
{
    public static class MappingExtensions
    {
        public static TApiModel ToApiModel<TSource, TApiModel>(this TSource source)
            where TSource : class
            where TApiModel : ApiModelBase
        {
            return SimpleMapper.Map<TSource, TApiModel>(source);
        }
    }
}