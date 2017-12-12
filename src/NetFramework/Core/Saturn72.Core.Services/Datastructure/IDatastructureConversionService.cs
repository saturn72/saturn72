#region Usings

using System.Threading.Tasks;

#endregion

namespace Saturn72.Core.Services.Datastructure
{
    public interface IDatastructureConversionService
    {
        Task<object> Convert(DatastructureType sourceDatastructure, DatastructureType destinationDatastructure, byte[] bytes);
    }
}