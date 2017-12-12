#region

using System;
using System.Collections.Generic;
using Saturn72.Module.EmailClients.Objects;

#endregion

namespace Saturn72.Module.EmailClients.Domain
{
    public class EmailMessage
    {
        public long Id { get; set; }

        public IEnumerable<EmailAddress> To { get; set; }
        
        public IEnumerable<EmailAddress> Bcc { get; set; }
        
        public MessageBody MessageBody { get; set; }

        public DateTime SentOn { get; set; }

        public bool Read { get; set; }
        public string Subject { get; set; }
    }
}