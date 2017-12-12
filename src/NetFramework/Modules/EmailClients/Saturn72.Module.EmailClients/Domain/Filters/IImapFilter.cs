#region



#endregion

namespace Saturn72.Module.EmailClients.Domain.Filters
{
    public interface IImapFilter<in TFiltered>
    {
        bool Filter(TFiltered source);

        /// <summary>
        ///     Converts the object to imap query representation
        /// </summary>
        /// <returns>string</returns>
        string ToImapQuery();
    }
}