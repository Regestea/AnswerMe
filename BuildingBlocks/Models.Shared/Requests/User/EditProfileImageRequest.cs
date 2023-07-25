using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.User
{
    public class EditProfileImageRequest
    {
        public string? ProfileImageToken { get; set; }
    }
}
