#region

using System.Net.Http.Formatting;
using Saturn72.Core.Tasks;

#endregion

namespace Mimas.Framework.Infrastructure
{
    public class BootStrap : IStartupTask
    {
        public void Execute()
        {
            //Load System.Net.Http.Formatting
            var t = MediaTypeFormatterMatchRanking.MatchOnCanWriteType;
        }

        public int ExecutionIndex
        {
            get { return -100; }
        }
    }
}