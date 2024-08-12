using System.ComponentModel.DataAnnotations;
using Models.Shared.Requests.Upload;

namespace Models.Shared.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileAllowed : ValidationAttribute
    {
        private readonly string _fileTypePropertyName;

        public FileAllowed(string fileTypePropertyName)
        {
            _fileTypePropertyName = fileTypePropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string) return new ValidationResult($"Invalid file format");
            var property = validationContext.ObjectType.GetProperty(_fileTypePropertyName);
            if (property == null)
                return new ValidationResult($"Unknown property: {_fileTypePropertyName}");

            var dependentValue = property.GetValue(validationContext.ObjectInstance, null)?.ToString();
            if (!Enum.TryParse(dependentValue, true, out FileType fileType))
                return new ValidationResult($"Invalid file format");
            bool validationField;
            
            switch (fileType)
            {
                case FileType.audio:
                    string[] audioExtensions = ["mp3", "ogg", "wav"];
                    validationField= !audioExtensions.Contains(value);
                    break;
                case FileType.image:
                    string[] imageExtensions = ["jpg", "jpeg", "png", "gif"];
                    validationField= !imageExtensions.Contains(value);
                    break;
                case FileType.profile:
                    string[] profileExtensions = ["jpg", "jpeg", "png", "gif"];
                    validationField= !profileExtensions.Contains(value);
                    break;
                case FileType.video:
                    string[] videoExtensions = ["mp4", "mov", "mkv"];
                    validationField= !videoExtensions.Contains(value);
                    break;
                case FileType.other:
                    string[] otherExtensions = ["rar", "zip", "pdf", "doc", "docx", "pdf", "xls", "xlsx"];
                    validationField= !otherExtensions.Contains(value);
                    break;
                default:
                    validationField = true;
                    break;
            }

            return validationField ? new ValidationResult($"Invalid file format") : ValidationResult.Success;

        }
    }
}
