using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.User
{
    public class UserResponse
    {
        public Guid id { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
        public string? FullName { get; set; }
        public string? IdName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
    }
}
