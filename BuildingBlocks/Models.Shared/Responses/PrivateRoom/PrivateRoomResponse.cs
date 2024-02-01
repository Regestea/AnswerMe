using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace Models.Shared.Responses.PrivateRoom
{
    public class PrivateRoomResponse
    {
        public PreviewUserResponse Contact { get; set; } = null!;
        
        public RoomNotifyResponse RoomNotify { get; set; }
    }
}
