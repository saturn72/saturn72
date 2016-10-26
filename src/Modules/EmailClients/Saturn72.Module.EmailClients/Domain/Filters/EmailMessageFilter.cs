#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.EmailClients.Domain.Filters
{
    public class EmailMessageFilter : IImapFilter<EmailMessage>
    {
        public long Id { get; set; }

        public IEnumerable<long> IncludedIdsList { get; set; }

        public IEnumerable<long> ExcludedIdsList { get; set; }

        public DateTime BeforeDateTime { get; set; }

        public DateTime OnDateTime { get; set; }

        public DateTime SinceDateTime { get; set; }

        public string From { get; set; }

        public bool? Read { get; set; }

        public bool Filter(EmailMessage source)
        {
            return CheckEmailMessageId(source.Id)
                   && CheckSentOnCriteria(source.SentOn)
                   && (Read.IsNull() ? true : source.Read == Read.Value);
        }

        public string ToImapQuery()
        {
            var query = new StringBuilder(AddDateTimeFilters());
            if (Id.NotDefault())
                query.Append(" UID " + Id);

            if (From.NotDefault())
                query.Append(" FROM " + From);

            if (Read.NotNull())
                query.Append(Read.Value ? " SEEN" : " UNSEEN");

            return query.ToString();
        }

        private string AddDateTimeFilters()
        {
            const string dateTimeFormat = "dd-MMM-yyyy";

            if (OnDateTime.NotDefault())
                return "ON " + OnDateTime.ToString(dateTimeFormat);

            if (BeforeDateTime.NotDefault())
                return "BEFORE " + BeforeDateTime.ToString(dateTimeFormat);

            if (SinceDateTime.NotDefault())
                return "SINCE " + SinceDateTime.ToString(dateTimeFormat);

            return string.Empty;
        }

        protected bool CheckEmailMessageId(long messageId)
        {
            return (Id != default(long) && messageId == Id)
                   || (IncludedIdsList.NotNull() && IncludedIdsList.Contains(messageId))
                   || (ExcludedIdsList.NotNull() && ExcludedIdsList.Contains(messageId));
        }

        protected bool CheckSentOnCriteria(DateTime sentOn)
        {
            if (sentOn.IsDefault())
                throw new ArgumentException("SentOn cannot be null. Please check imap message fetching method");

            if (OnDateTime.IsDefault()
                && BeforeDateTime.IsDefault()
                && SinceDateTime.IsDefault())
                return true;

            if (OnDateTime.NotDefault() && sentOn == OnDateTime)
                return true;

            if (BeforeDateTime.NotDefault() && sentOn < BeforeDateTime)
                return true;

            if (SinceDateTime.NotDefault() && sentOn > SinceDateTime)
                return true;

            return false;
        }
    }
}