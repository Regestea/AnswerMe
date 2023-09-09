using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.User
{
    public class RegisterUserRequest
    {

        [Required]
        [MaxLength(15)]
        public string IdName { get; set; } = null!;

        [Required]
        [StringLength(maximumLength: 13, MinimumLength = 10)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Password { get; set; } = null!;
    }
}
