using System.ComponentModel.DataAnnotations;

namespace Models.Shared.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileAllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public FileAllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string fileFormat)
            {
                if (string.IsNullOrEmpty(fileFormat) || !_extensions.Contains(fileFormat))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return "This Extension Is Not Allowed";
        }
    }
}
