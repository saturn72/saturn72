#region

using System;
using System.Diagnostics;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Core.Domain.Tasks
{
    public class BackgroundTaskExecutionDataDomainModel : DomainModelBase<long>
    {
        public BackgroundTaskExecutionDataDomainModel()
        {
            CreatedOnUtc = DateTime.UtcNow;
        }

        public DateTime CreatedOnUtc { get; set; }

        public string ErrorData { get; set; }

        public string OutputData { get; set; }

        public int ProcessId { get; set; }

        public string ProcessStartInfo { get; private set; }

        public DateTime? ProcessStartedOnUtc { get; set; }

        public DateTime? ProcessExitedOnUtc { get; set; }

        public long ProcessExitCode { get; set; }

        public string Exception { get; private set; }

        #region Methods

        public void SetPocessStartInfo(ProcessStartInfo startInfo)
        {
            ProcessStartInfo = startInfo.AsString();
        }

        public void SetException(Exception ex)
        {
            Exception = ex.AsString();
        }

        #endregion
    }
}