#region

using System;
using System.Collections.Generic;
using System.Linq;
using NCrontab.Advanced;
using NCrontab.Advanced.Enumerations;
using Saturn72.Common.Tasks;

#endregion

namespace Saturn72.Module.Tasks.NCrontabAdvanced
{
    public class NCronAdvancedCronValidator : ICronValidator
    {
        private readonly IEnumerable<CronStringFormat> _cronStringFromats =
            Enum.GetValues(typeof (CronStringFormat)).Cast<CronStringFormat>();

        public bool ValidateCronExpression(string cronExpression)
        {
            return _cronStringFromats.Any(csf => CrontabSchedule.TryParse(cronExpression, csf) !=
                                                 default(CrontabSchedule));
        }
    }
}