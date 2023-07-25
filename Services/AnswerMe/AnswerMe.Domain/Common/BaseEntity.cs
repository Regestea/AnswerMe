using System.ComponentModel.DataAnnotations;

namespace AnswerMe.Domain.Common
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
