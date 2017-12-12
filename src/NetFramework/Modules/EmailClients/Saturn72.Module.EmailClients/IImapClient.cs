#region

using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using Saturn72.Module.EmailClients.Domain;
using Saturn72.Module.EmailClients.Domain.Filters;

#endregion

namespace Saturn72.Module.EmailClients
{
    public interface IImapClient
    {
        string Host { get; }

        int Port { get; }

        SslProtocols SslProtocol { get; }

        bool ValidateServerCertificate { get; }

        bool Login(string username, string password);

        Task<IEnumerable<Folder>> GetFoldersAsync();

        IEnumerable<Folder> GetFolders();

        Task<IEnumerable<Folder>> GetAllIncomingMessagesByFolderAsync(EmailMessageFilter filter = null);

        IEnumerable<Folder> GetAllIncomingMessagesByFolder(EmailMessageFilter filter = null);

        IEnumerable<EmailMessage> GetAllIncomingMessages(EmailMessageFilter filter = null);

        void GetFolderMessages(Folder folder, EmailMessageFilter filter);
    }
}