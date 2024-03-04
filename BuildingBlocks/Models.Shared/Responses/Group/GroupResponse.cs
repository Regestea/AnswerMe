using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace Models.Shared.Responses.Group
{
    public class GroupResponse
    {
        public PreviewGroupResponse Group { get; set; }

        public RoomNotifyResponse RoomNotify { get; set; }
    }
}
