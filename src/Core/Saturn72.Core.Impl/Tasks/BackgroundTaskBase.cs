#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Tasks;

#endregion

namespace Saturn72.Core.Services.Impl.Tasks
{
    public abstract class BackgroundTaskBase : ITask
    {
        private static readonly string NewLine = Environment.NewLine;
        private IEventPublisher _eventPublisher;
        private IDictionary<string, object> _parameters;
        private BackgroundTaskExecutionDataDomainModel _ted;

        protected virtual IEventPublisher EventPublisher
        {
            get
            {
                return _eventPublisher ??
                       (_eventPublisher = AppEngine.Current.Resolve<IEventPublisher>());
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
            _ted = new BackgroundTaskExecutionDataDomainModel();
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
                HandleException(ex, proc);
            }

            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            proc.WaitForExit();
            WriteOutputData("ProcessExitCode: " + proc.ExitCode);

            if (proc.ExitCode != 0)
            {
                var ex = new Exception("Background task failed with exit code: " + proc.ExitCode + NewLine +
                                       GeneralExceptionMessage);
                HandleException(ex, proc);
            }
            _ted.ProcessExitedOnUtc = proc.ExitTime.ToUniversalTime();
            PublishCreateEvent();
            proc.Close();
        }

        private void PublishCreateEvent()
        {
            EventPublisher.DomainModelCreated<BackgroundTaskExecutionDataDomainModel, long>(_ted);
        }

        private static void WriteErrorData(string data, BackgroundTaskExecutionDataDomainModel ted)
        {
            ted.ErrorData += NewLine + data;
            DefaultOutput.WriteLine(data);
        }

        private void WriteOutputData(string data)
        {
            _ted.OutputData += NewLine + data;
            DefaultOutput.WriteLine(data);
        }

        private void HandleException(Exception ex, Process process)
        {
            DefaultOutput.WriteLine(ex);
            _ted.SetException(ex);

            try
            {
                _ted.ProcessExitCode = process.ExitCode;
                _ted.ProcessExitedOnUtc = process.ExitTime.ToUniversalTime();

            }
            catch
            {
                // ignored
            }
            PublishCreateEvent();
            throw ex;
        }

        public abstract ProcessStartInfo GetProcessStartInfo();
    }
}