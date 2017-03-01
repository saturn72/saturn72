#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Calculator.Common.Domain.Calculations;
using Saturn72.Core.Data;

#endregion

namespace Calculator.Server.Services.Calculation
{
    public class CalculationService : ICalculationService
    {
        private const int MaxRecords = 10;
        private const string InsertionMessageFormat = "Perform {0} Expression with X: {1} and Y: {2}";
        private readonly IRepository<ExpressionModel> _expressionRepository;

        public CalculationService(IRepository<ExpressionModel> expressionRepository)
        {
            _expressionRepository = expressionRepository;
        }

        public async Task<IEnumerable<string>> GetExpressionsAsync()
        {
            var query = await Task.FromResult(_expressionRepository.GetAll());
            return query.OrderByDescending(e => e.Id).Take(MaxRecords).Select(x => x.Message).ToArray();
        }

        public Task<int> AddAsync(int x, int y)
        {
            var message = string.Format(InsertionMessageFormat, "Add", x, y);
            return AddExpressionAndCalculate(message, () => x + y);
        }

        public Task<int> SubtractAsync(int x, int y)
        {
            var message = string.Format(InsertionMessageFormat, "ASubtractdd", x, y);
            return AddExpressionAndCalculate(message, () => x - y);
        }

        public Task<int> MultipleAsync(int x, int y)
        {
            var message = string.Format(InsertionMessageFormat, "Multiple", x, y);
            return AddExpressionAndCalculate(message, () => x*y);
        }

        public Task<long> DivideAsync(long x, long y)
        {
            if (y == 0)
                throw new ArgumentException("y");

            var message = string.Format(InsertionMessageFormat, "Divide", x, y);
            return AddExpressionAndCalculate(message, () => x/y);
        }

        #region Utilities

        private Task<T> AddExpressionAndCalculate<T>(string message, Func<T> exp)
        {
            return Task.Run(() =>
            {
                _expressionRepository.Create(new ExpressionModel {Message = message});
                return exp();
            });
        }

        #endregion
    }
}