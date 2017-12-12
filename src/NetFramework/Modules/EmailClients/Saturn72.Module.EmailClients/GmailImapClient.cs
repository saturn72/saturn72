#region

using System.Security.Authentication;

#endregion

namespace Saturn72.Module.EmailClients
{
    public sealed class GmailImapClient : ImapClientBase
    {
        private readonly string[] _nonInboxFolderPaths = { "[Gmail]", "[Gmail]/All Mail", "[Gmail]/Spam", "[Gmail]/Trash", "[Gmail]/Chats", "[Gmail]/Sent Mail", "[Gmail]/Drafts", "[Gmail]/Important" };

        public GmailImapClient(IImapCommander imapCommander) : base(imapCommander)
        {
        }

        public override string[] NonInboxFolderPaths
        {
            get { return _nonInboxFolderPaths; }
        }

        public override string Host
        {
            get { return "imap.gmail.com"; }
        }

        public override int Port
        {
            get { return 993; }
        }

        public override SslProtocols SslProtocol
        {
            get { return SslProtocols.Ssl3; }
        }

        public override bool ValidateServerCertificate
        {
            get { return true; }
        }
    }
}