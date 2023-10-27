using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Api.Entities
{
    public class User:BaseEntity
    {

        [Required]
        [MaxLength(50)]
        public string IdName { get; set; } = null!;

        [Required]
        [StringLength(maximumLength: 13, MinimumLength = 10)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
