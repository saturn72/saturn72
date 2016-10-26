using System;
using System.Threading;

namespace Saturn72.Extensions
{
    public class Toolbox
    {
        /// <summary>
        /// Runs expression with timeout flag
        /// </summary>
        /// <param name="expression">Expression to examined</param>
        public static bool RunTimedoutExpression(Func<bool> expression)
        {
            return RunTimedoutExpression(expression, 5000);
        }

        /// <summary>
        /// Runs expression with timeout flag
        /// </summary>
        /// <param name="expression">Expression to examined</param>
        /// <param name="totalMiliSecTimeout">Total time out in milisecs</param>
        public static bool RunTimedoutExpression(Func<bool> expression, int totalMiliSecTimeout)
        {
            return RunTimedoutExpression(expression, totalMiliSecTimeout, 50);
        }

        /// <summary>
        /// Runs expression with timeout flag
        /// </summary>
        /// <param name="expression">Expression to examined</param>
        /// <param name="totalMiliSecTimeout">Total time out</param>
        /// <param name="milisecInterval">Sleep interval between expression execution</param>
        public static bool RunTimedoutExpression(Func<bool> expression, int totalMiliSecTimeout, int milisecInterval)
        {
            bool result;

            while (!(result = expression()))
            {
                Thread.Sleep(milisecInterval);
                totalMiliSecTimeout -= milisecInterval;

                if(totalMiliSecTimeout <= 0)
                    return false;
            }
            
            return result;
        }
    }
}
