using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Api.Entities
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            CreatedDate ??= DateTimeOffset.UtcNow;
        }

        [Key]
        public Guid id { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}
