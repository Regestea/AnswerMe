using System.ComponentModel.DataAnnotations;

namespace Models.Shared.Requests.Group
{
    public class EditGroupRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? ImageToken { get; set; }
    }
}
