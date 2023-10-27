namespace IdentityServer.Api.DTOs
{
    public class BaseEntityDto
    {
        public Guid id { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
