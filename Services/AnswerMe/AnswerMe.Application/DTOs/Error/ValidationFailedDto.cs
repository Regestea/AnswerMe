namespace AnswerMe.Application.DTOs.Error
{
    public class ValidationFailedDto
    {
        public string Field { get; set; } = null!;
        public string Error { get; set; } = null!;
    }
}