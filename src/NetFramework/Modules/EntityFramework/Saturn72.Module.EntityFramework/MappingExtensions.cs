#region

using System.Collections.Generic;
using System.Linq;
using Saturn72.Mappers;

#endregion

namespace Saturn72.Module.EntityFramework
{
    internal static class MappingExtensions
    {
        public static IEnumerable<TDomainModel> ToDomainModel<TEntity, TDomainModel>(this IEnumerable<TEntity> source)
            where TDomainModel : class
            where TEntity : class
        {
            return source
                .Select(ToDomainModel<TEntity, TDomainModel>)
                .ToArray();
        }

        public static TDomainModel ToDomainModel<TEntity, TDomainModel>(this TEntity entity)
            where TDomainModel : class
            where TEntity : class
        {
            return SimpleMapper.Map<TEntity, TDomainModel>(entity);
        }

        public static TEntity ToEntity<TDomainModel, TEntity>(this TDomainModel model)
            where TEntity : class
            where TDomainModel : class
        {
            return SimpleMapper.Map<TDomainModel, TEntity>(model);
        }

        public static TDestination MapToInstance<TSource, TDestination>(this TSource source, TDestination destination)
            where TDestination : class
            where TSource : class
        {
            return SimpleMapper.Map(source, destination);
        }
    }
}