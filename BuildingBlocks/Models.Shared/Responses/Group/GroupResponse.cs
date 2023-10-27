using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Group
{
    public class GroupResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? RoomImage { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }


    }
}
