namespace Saturn72.Core.Domain.Clients
{
    public class ClientAppDomainModel : DomainModelBase<long>
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool Active { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }
        public string RowVersion { get; set; }
    }
}