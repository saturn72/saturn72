#region

using System.Collections.Generic;
using System.Security.Authentication;
using Saturn72.Module.EmailClients.Domain;

#endregion

namespace Saturn72.Module.EmailClients
{
    public interface IImapCommander
    {
        bool IsConnected { get; }
        bool Login(string username, string password);
        bool Connect(string host, int port, SslProtocols sslProtocol, bool validateServerCertificate);
        IEnumerable<Folder> GetFolders();

        IEnumerable<EmailMessage> GetFolderMessages(Folder folder, string query);
    }
}