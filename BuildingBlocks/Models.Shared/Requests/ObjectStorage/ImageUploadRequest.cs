using System.ComponentModel.DataAnnotations;
using Models.Shared.Attribute;

namespace Models.Shared.Requests.ObjectStorage
{
    public class ImageUploadRequest
    {
        [Required]
        [Range(0.01, 20)]
        public double FileSizeMB { get; set; }

        [MaxLength(36, ErrorMessage = "the component size should be 4 x 4 and string length 36")]
        [MinLength(36, ErrorMessage = "the component size should be 4 x 4 and string length 36")]
        [Required]
        public string BlurHash { get; set; }=null!;

        [FileAllowedExtensions("jpg", "jpeg", "png", "gif")]
        [Required]
        public string FileFormat { get; set; } = null!;

    }
}
