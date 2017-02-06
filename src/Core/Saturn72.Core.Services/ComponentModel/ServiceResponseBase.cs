using System.Collections.Generic;
using System.Linq;

namespace Saturn72.Core.Services.ComponentModel
{
    public abstract class ServiceRequestBase<T> where T : class
    {
        private IEnumerable<string> _errorMessages;
        public T Result { get; protected set; }
        public bool IsValid => ErrorMessages.Any();

        public virtual IEnumerable<string> ErrorMessages
        {
            get { return _errorMessages ?? (_errorMessages = new List<string>()); }
            protected set { _errorMessages = value; }
        }

        protected void AddErrorMessage(string errorMessage)
        {
            ((ICollection<string>) ErrorMessages).Add(errorMessage);
        }
    }
}