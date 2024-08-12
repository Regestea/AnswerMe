namespace Models.Shared.Responses.Group;

public class PreviewGroupInviteResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? RoomImage { get; set; }

    public DateTimeOffset? CreatedDate { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
}