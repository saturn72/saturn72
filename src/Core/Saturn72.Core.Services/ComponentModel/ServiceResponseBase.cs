using System.Collections.Generic;
using System.Linq;

namespace Saturn72.Core.Services.ComponentModel
{
    public abstract class ServiceResponseBase
    {
        private IEnumerable<string> _errorMessages;
        private bool _hasErrors;
        private bool _authorized = true;

        public virtual bool HasErrors
        {
            get { return _hasErrors || ErrorMessages.Any(); }
            set { _hasErrors = value; }
        }

        public virtual IEnumerable<string> ErrorMessages
        {
            get { return _errorMessages ?? (_errorMessages = new List<string>()); }
            protected set { _errorMessages = value; }
        }

        public virtual bool Authorized
        {
            get { return _authorized; }
            set
            {
                _authorized = value;
                if (!value)
                    AddErrorMessage("Unauthorized request");
            }
        }

        public void AddErrorMessage(string errorMessage)
        {
            ((ICollection<string>)ErrorMessages).Add(errorMessage);
        }
    }
}