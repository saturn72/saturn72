#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Saturn72.Extensions;
using Saturn72.Module.EmailClients.Domain;
using Saturn72.Module.EmailClients.Domain.Filters;

#endregion

namespace Saturn72.Module.EmailClients
{
    public abstract class ImapClientBase : IImapClient
    {
        private readonly IImapCommander _imapCommander;

        protected ImapClientBase(IImapCommander imapCommander)
        {
            _imapCommander = imapCommander;
        }

        /// <summary>
        ///     Gets array of all folders names that not representing inbox folders.
        ///     <remarks>Folder names in this collection are execluded from GetIncomingMessages search</remarks>
        /// </summary>
        public abstract string[] NonInboxFolderPaths { get; }

        public abstract string Host { get; }
        public abstract int Port { get; }
        public abstract SslProtocols SslProtocol { get; }
        public abstract bool ValidateServerCertificate { get; }

        public bool Login(string username, string password)
        {
            return ConnectAndRun(() => _imapCommander.Login(username, password));
        }

        public Task<IEnumerable<Folder>> GetFoldersAsync()
        {
            return new Task<IEnumerable<Folder>>(GetFolders);
        }

        public IEnumerable<Folder> GetFolders()
        {
            return _imapCommander.GetFolders();
        }

        public Task<IEnumerable<Folder>> GetAllIncomingMessagesByFolderAsync(EmailMessageFilter filter = null)
        {
            return new Task<IEnumerable<Folder>>(() => GetAllIncomingMessagesByFolder(filter));
        }

        public IEnumerable<Folder> GetAllIncomingMessagesByFolder(EmailMessageFilter filter = null)
        {
            var folders = GetFolders().ToArray();
            var incomingFolders = new List<Folder>();
            GetIncomingFoldersOnly(folders, incomingFolders);

            filter = filter ?? (new EmailMessageFilter());
            filter.Read = false;

            incomingFolders.ForEach(f => GetFolderMessages(f, filter));

            return folders;
        }

        public IEnumerable<EmailMessage> GetAllIncomingMessages(EmailMessageFilter filter = null)
        {
            var folders = GetAllIncomingMessagesByFolder(filter);
            var incomingMessages = new List<EmailMessage>();

            FlatMessagesFromFolders(folders, incomingMessages);

            return filter.Filter(incomingMessages).ToArray();
        }

        public void GetFolderMessages(Folder folder, EmailMessageFilter filter)
        {
            var query = filter.ToImapQuery();
            _imapCommander.GetFolderMessages(folder, query);
        }

        private void FlatMessagesFromFolders(IEnumerable<Folder> folders,
            List<EmailMessage> flattenMessagesCollection)
        {
            foreach (var f in folders)
            {
                if (f.Messages.NotEmptyOrNull())
                    flattenMessagesCollection.AddRange(f.Messages);
                if (f.SubFolders.NotEmptyOrNull())
                    FlatMessagesFromFolders(f.SubFolders, flattenMessagesCollection);
            }
        }

        private void GetIncomingFoldersOnly(IEnumerable<Folder> folders, List<Folder> result)
        {
            foreach (var folder in folders)
            {
                if (folder.SubFolders.Any())
                    GetIncomingFoldersOnly(folder.SubFolders, result);

                if (!NonInboxFolderPaths.Contains(folder.Path))
                    result.Add(folder);
            }
        }

        private bool ConnectAndRun(Func<bool> func)
        {
            if (!
                _imapCommander.IsConnected)
                Connect();
            return func();
        }

        public bool Connect()
        {
            return _imapCommander.Connect(Host, Port, SslProtocol, ValidateServerCertificate);
        }
    }
}