namespace AnswerMe.Client.Core.DTOs.User
{
    public class UserDto
    {
        public Guid id { get; set; }
        public string IdName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
