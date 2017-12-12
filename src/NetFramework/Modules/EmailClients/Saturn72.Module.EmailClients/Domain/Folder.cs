using System.Collections.Generic;
using Saturn72.Module.EmailClients.Objects;

namespace Saturn72.Module.EmailClients.Domain
{
    public class Folder:ImapObjectBase
    {
        public string UId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool HasChildren { get; set; }
        public bool IsReadOnly { get; set; }
        public IEnumerable<Folder> SubFolders { get; set; }
        public IEnumerable<EmailMessage> Messages { get; set; }
    }
}
