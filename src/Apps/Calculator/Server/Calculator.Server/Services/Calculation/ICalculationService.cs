#region

using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace Calculator.Server.Services.Calculation
{
    public interface ICalculationService
    {
        Task<IEnumerable<string>> GetExpressionsAsync();

        Task<int> AddAsync(int x, int y);
        Task<int> SubtractAsync(int x, int y);
        Task<int> MultipleAsync(int x, int y);
        Task<long> DivideAsync(long x, long y);
    }
}