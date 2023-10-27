using Models.Shared.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.ObjectStorage
{
    public class AudioUploadRequest
    {
        [Required]
        [Range(0.01,100)]
        public double FileSizeMB { get; set; }

        [Required]
        [FileAllowedExtensions("mp3", "ogg", "wav")]
        public string FileFormat { get; set; } = null!;
    }
}
