using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.User
{
    public class EditUserRequest
    {
        [MaxLength(50)]
        public string? FullName { get; set; } 
        public string? ProfileImageToken { get; set; }
    }
}
