#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using ImapX;
using Saturn72.Module.EmailClients.Domain;
using Folder = Saturn72.Module.EmailClients.Domain.Folder;
using MessageBody = Saturn72.Module.EmailClients.Domain.MessageBody;

#endregion

namespace Saturn72.Module.EmailClients.ImapXCommander
{
    public class ImapxImapCommander : IImapCommander
    {
        private readonly ImapClient _client;

        public ImapxImapCommander()
        {
            _client = new ImapClient();
        }

        public bool Login(string username, string password)
        {
            return _client.Login(username, password);
        }

        public bool Connect(string host, int port, SslProtocols sslProtocol, bool validateServerCertificate)
        {
            var useSsl = Equals(sslProtocol, SslProtocols.Ssl2)
                         || Equals(sslProtocol, SslProtocols.Ssl3);

            return _client.Connect(host, port, useSsl, validateServerCertificate);
        }

        public IEnumerable<Domain.Folder> GetFolders()
        {
            return _client.Folders.Select(ToImapFolder).ToArray();
        }

        public IEnumerable<EmailMessage> GetFolderMessages(Domain.Folder folder, string query)
        {
            var pathArray = folder.Path.Split('/');
            var remoteFolder = _client.Folders[pathArray[0]];
            for (var i = 1; i < pathArray.Length; i++)
                remoteFolder = remoteFolder.SubFolders[pathArray[i]];

            folder.Messages = remoteFolder.Search(query).Select(ToEmailMessage).ToArray();
            return folder.Messages;
        }

        public bool IsConnected
        {
            get { return _client.IsConnected; }
        }

        private Domain.Folder ToImapFolder(ImapX.Folder folder)
        {
            var subFolders = folder.SubFolders
                .Select(ToImapFolder)
                .ToArray();

            return new Domain.Folder
            {
                UId = folder.UidValidity + "_" + folder.UidNext,
                Name = folder.Name,
                Path = folder.Path,
                HasChildren = folder.HasChildren,
                IsReadOnly = folder.ReadOnly,
                SubFolders = subFolders,
            };
        }


        private EmailMessage ToEmailMessage(Message message)
        {
            var to = message.To.Select(t => new EmailAddress {Address = t.Address, DisplayName = t.DisplayName})
                .ToArray();

            var bcc = message.Bcc.Select(t => new EmailAddress {Address = t.Address, DisplayName = t.DisplayName})
                .ToArray();

            var body = new Domain.MessageBody
            {
                HtmlContent = message.Body.Html,
                TextContent = message.Body.Text
            };

            return new EmailMessage
            {
                Subject = message.Subject,
                To = to,
                Bcc = bcc,
                MessageBody = body,
                Read = message.Seen,
                Id = message.UId,
                SentOn = message.Date ?? default(DateTime)
            };
        }

        /*public Attachment[] Attachments { get; }
        public MessageBody Body { get; set; }
        public MessageContent[] BodyParts { get; }
        public List<MailAddress> Cc { get; set; }
        public string Comments { get; set; }
        public string ContentTransferEncoding { get; set; }
        public ContentType ContentType { get; set; }
        public DateTime? Date { get; set; }
        public Attachment[] EmbeddedResources { get; }
        public MessageFlagCollection Flags { get; }
        public MailAddress From { get; set; }
        public long? GMailMessageId { get; }
        public GMailMessageThread GmailThread { get; }
        public Dictionary<string, string> Headers { get; set; }
        public MessageImportance Importance { get; set; }
        public string InReplyTo { get; set; }
        public DateTime? InternalDate { get; }
        public GMailMessageLabelCollection Labels { get; }
        public string Language { get; set; }
        public string Mailer { get; set; }
        public string MessageId { get; set; }
        public string MimeVersion { get; set; }
        public string Organization { get; set; }
        public List<MailAddress> ReplyTo { get; set; }
        public MailAddress ReturnPath { get; set; }
        public bool Seen { get; set; }
        public MailAddress Sender { get; set; }
        public MessageSensitivity Sensitivity { get; set; }
        public long SequenceNumber { get; set; }
        public long Size { get; }
        public string Subject { get; set; }
        public List<MailAddress> To { get; set; }
        public long UId { get; }

        public override byte[] AppendCommandData(string serverResponse);
        public bool CopyTo(Folder folder, bool downloadCopy = false);
        public bool Download(MessageFetchMode mode = MessageFetchMode.ClientDefault, bool reloadHeaders = false);
        public string DownloadRawMessage();
        public static Message FromEml(string eml);
        public bool MoveTo(Folder folder, bool downloadCopy = false);
        public override void ProcessCommandResult(string data);
        public bool Remove();
        public void Save(string filePath);
        public void SaveTo(string folderPath, string fileName);
        public string ToEml();*/
        // }
    }
}