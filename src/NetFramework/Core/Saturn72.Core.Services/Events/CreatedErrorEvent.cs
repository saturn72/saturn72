#region Usings

using System;
using Saturn72.Core.Domain;

#endregion

namespace Saturn72.Core.Services.Events
{
    public class CreatedErrorEvent<TDomainModel> : CrudEventBase<TDomainModel> where TDomainModel : DomainModelBase
    {
        public CreatedErrorEvent(TDomainModel domainModel, string errorMessage, Exception exception) : base(domainModel)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
        }

        public Exception Exception { get; }

        public string ErrorMessage { get; }
    }
}