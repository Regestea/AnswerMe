using Models.Shared.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.ObjectStorage
{
    public class OtherUploadRequest
    {
        [Required]
        [Range(0.01, 2048)]
        public double FileSizeMB { get; set; }

        [Required]
        [FileAllowedExtensions("rar", "zip", "pdf", "doc", "docx", "pdf", "xls", "xlsx")]
        public string FileFormat { get; set; } = null!;
    }
}
