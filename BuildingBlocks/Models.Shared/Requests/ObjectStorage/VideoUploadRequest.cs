using System.ComponentModel.DataAnnotations;
using Models.Shared.Attribute;

namespace Models.Shared.Requests.ObjectStorage
{
    public class VideoUploadRequest
    {
        [Required]
        [Range(0.01, 2048)]
        public double FileSizeMB { get; set; }

        [Required]
        [FileAllowedExtensions("mp4", "mov", "mkv")]
        public string FileFormat { get; set; } = null!;
    }
}
