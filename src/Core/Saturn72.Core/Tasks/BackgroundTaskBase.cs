#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;

#endregion

namespace Saturn72.Core.Tasks
{
    public abstract class BackgroundTaskBase : ITask
    {
        private static readonly string NewLine = Environment.NewLine;
        private IDictionary<string, object> _parameters;
        private BackgroundTaskExecutionDomainModel _ted;
        private IBackgroundTaskExecutionService _taskExecutionService;

        protected virtual IBackgroundTaskExecutionService BackgroundTaskExecutionService
        {
            get
            {
                _taskExecutionService ??
                (_taskExecutionService ?? AppEngine.Current.Resolve<IBackgroundTaskExecutionService>());
            }
        }

        public IDictionary<string, object> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, object>()); }
            set { _parameters = value; }
        }

        public virtual string GeneralExceptionMessage
        {
            get { return string.Empty; }
        }

        public virtual void Execute()
        {
            _ted = new BackgroundTaskExecutionDomainModel();
            var proc = new Process();

            proc.OutputDataReceived += (sender, e) => WriteOutputData(e.Data);

            proc.ErrorDataReceived += (sender, e) => WriteErrorData(e.Data, _ted);

            proc.StartInfo = GetProcessStartInfo();
            _ted.SetPocessStartInfo(proc.StartInfo);

            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;

            try
            {
                proc.Start();
                _ted.ProcessId = proc.Id;
                _ted.ProcessStartedOnUtc = proc.StartTime.ToUniversalTime();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            proc.WaitForExit();
            WriteOutputData("ExitCode: " + proc.ExitCode);

            if (proc.ExitCode != 0)
            {
                var ex = new Exception("Background task failed with exit code: " + proc.ExitCode + NewLine +
                                       GeneralExceptionMessage);
                HandleException(ex);
            }
            _ted.ProcessExitedOnUtc = proc.ExitTime.ToUniversalTime();

            proc.Close();
        }

        private static void WriteErrorData(string data, BackgroundTaskExecutionDomainModel ted)
        {
            ted.ErrorData += NewLine + data;
            DefaultOutput.WriteLine(data);
        }

        private void WriteOutputData(string data)
        {
            _ted.OutputData += NewLine + data;
            DefaultOutput.WriteLine(data);
        }

        private void HandleException(Exception ex)
        {
            _ted.SetException(ex);
            DefaultOutput.WriteLine(ex);
            throw ex;
        }

        public abstract ProcessStartInfo GetProcessStartInfo();
    }
}