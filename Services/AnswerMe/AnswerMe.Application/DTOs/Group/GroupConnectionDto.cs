namespace AnswerMe.Application.DTOs.Group
{
    public class GroupConnectionDto
    {
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; } = null!;
        public Guid GroupId { get; set; }
    }
}
