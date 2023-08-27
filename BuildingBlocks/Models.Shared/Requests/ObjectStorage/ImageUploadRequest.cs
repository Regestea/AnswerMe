using System.ComponentModel.DataAnnotations;
using Models.Shared.Attribute;

namespace Models.Shared.Requests.ObjectStorage
{
    public class ImageUploadRequest
    {
        [Required]
        [Range(0.01, 20)]
        public double FileSizeMB { get; set; }

        [FileAllowedExtensions("jpg", "jpeg", "png", "gif")]
        [Required]
        public string FileFormat { get; set; } = null!;

    }
}
