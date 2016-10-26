#region

using System.Collections.Generic;

#endregion

namespace Saturn72.Module.EmailClients
{
    public class ImapManager
    {
        private static ICollection<IImapClient> _imapClients;

        public static ICollection<IImapClient> ImapClients
        {
            get { return _imapClients ?? (_imapClients = new List<IImapClient>()); }
        }
    }
}