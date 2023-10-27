using System.ComponentModel.DataAnnotations;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class User : BaseEntity
    {

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string IdName { get; set; } = null!;

        [Required]
        [StringLength(maximumLength: 13, MinimumLength = 10)]
        public string PhoneNumber { get; set; } = null!;

        public string? ProfileImage { get; set; }
    }
}
