namespace Models.Shared.Responses.User
{
    public class PreviewUserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ProfileImage { get; set; }

        public bool IsOnline { get; set; }
    }
}
