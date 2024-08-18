using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class Media:BaseEntity
    {
        [Required]
        public MediaType Type { get; set; }

        [Required]
        public string FileName { get; set; } = null!;
        
        [Required]
        public string Path { get; set; } = null!;

        public string? BlurHash { get; set; }

        [ForeignKey("Message")]
        public Guid MessageId { get; set; }

        public Message Message { get; set; } = null!;
    }
}
