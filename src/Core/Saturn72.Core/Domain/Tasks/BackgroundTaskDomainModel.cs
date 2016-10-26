#region

using System;
using System.Collections.Generic;
using Saturn72.Core.Audit;

#endregion

namespace Saturn72.Core.Domain.Tasks
{
    public class BackgroundTaskDomainModel : DomainModelBase<long>, IUpdatedAudit
    {
        #region Fields

        private IDictionary<string, string> _parameters;
        private ICollection<BackgroundTaskAttachtmentDomainModel> _attachtments;

        #endregion

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Cron { get; set; }
        public bool Active { get; set; }
        public bool StopOnError { get; set; }
        public string TaskTypeUniqueIdentifier { get; set; }

        public virtual IDictionary<string, string> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, string>()); }
            set { _parameters = value; }
        }

        public DateTime? LastStartedOnUtc { get; set; }
        public DateTime? LastEndedOnUtc { get; set; }
        public DateTime? LastSuccessOnUtc { get; set; }
        public string Description { get; set; }
        public string AdminComment { get; set; }
        public TimeSpan DelayTimeSpan { get; set; }
        public long Seconds { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public ICollection<BackgroundTaskAttachtmentDomainModel> Attachtments
        {
            get { return _attachtments ?? (_attachtments = new List<BackgroundTaskAttachtmentDomainModel>()); }
            set { _attachtments = value; }
        }
    }
}