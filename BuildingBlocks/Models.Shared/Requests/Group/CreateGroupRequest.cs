using System.ComponentModel.DataAnnotations;

namespace Models.Shared.Requests.Group
{
    public class CreateGroupRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? ImageToken { get; set; }
    }
}
