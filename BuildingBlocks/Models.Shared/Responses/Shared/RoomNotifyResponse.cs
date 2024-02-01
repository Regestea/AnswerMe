namespace Models.Shared.Responses.Message;

public class RoomNotifyResponse
{
    public Guid RoomId { get; set; }
    public int TotalUnRead { get; set; }
    public string MessageGlance { get; set; }
}