using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.Group
{
    public class CreateInviteTokenRequest
    {
        [Required]
        [Range(1,int.MaxValue)]
        public int UserCount { get; set; }

        [Required]
        [Range(1,int.MaxValue)]
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
