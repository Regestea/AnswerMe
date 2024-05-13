namespace Models.Shared.Responses.Group;

public class RoomResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? RoomImage { get; set; }
}