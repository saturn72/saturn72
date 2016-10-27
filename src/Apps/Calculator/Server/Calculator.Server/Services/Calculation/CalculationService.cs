#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace Calculator.Server.Services.Calculation
{
    public class CalculationService : ICalculationService
    {
        private const int MaxSize = 10;
        private const string InsertionMessageFormat = "Perform {0} Expression with X: {1} and Y: {2}";

        private static string[] AllExpressions;

        private static int InsertionIndex;

        static  CalculationService()
        {
            CreateOrResetMessageCollection();
        }
        public Task<IEnumerable<string>> GetExpressionsAsync()
        {
            return Task.FromResult(AllExpressions as IEnumerable<string>);
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

        public static void CreateOrResetMessageCollection()
        {
            InsertionIndex = 0;
            AllExpressions = new string[MaxSize];
        }

        #region Utilities

        private Task<T> AddExpressionAndCalculate<T>(string message, Func<T> exp)
        {
            return Task.Run(() =>
            {
                AllExpressions[InsertionIndex] = message;
                InsertionIndex = ++InsertionIndex%10;
                return exp();
            });
        }

        #endregion
    }
}