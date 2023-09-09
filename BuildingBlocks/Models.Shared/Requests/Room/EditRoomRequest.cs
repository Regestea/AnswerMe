using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.Room
{
    public class EditRoomRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? ImageToken { get; set; }
    }
}
