#region

using System;
using System.Diagnostics;
using Saturn72.Core.Services.Impl.Tasks;

#endregion

namespace Saturn72.Common.Tasks
{
    public class SandboxedBackgroundTask : BackgroundTaskBase
    {
        public override string GeneralExceptionMessage { get; }

        public override ProcessStartInfo GetProcessStartInfo()
        {
            throw new NotImplementedException();
        }
    }
}