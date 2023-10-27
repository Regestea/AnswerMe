
using System.ComponentModel.DataAnnotations;

namespace Models.Shared.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FileSizeMegabyteAttribute : ValidationAttribute
    {
        private readonly double _minFileSize;
        private readonly double _maxFileSize;


        public FileSizeMegabyteAttribute(double minFileSize, double maxFileSize)
        {
            _minFileSize = minFileSize;
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var maxFileSize = _maxFileSize * 1024 * 1024;
            var minFileSize = _minFileSize * 1024 * 1024;

            if (value is Stream stream)
            {
                if (stream.Length > maxFileSize || stream.Length < minFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            else if (value is FileStream fileStream)
            {

                if (fileStream.Length > maxFileSize || fileStream.Length < minFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            else if (value is string base64File)
            {
                if (base64File.Length > maxFileSize || base64File.Length < minFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            else if (value is byte[] bytes)
            {

                if (bytes.Length >= maxFileSize || bytes.Length <= minFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }

            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return string.Format("Allowed file size is between {0} KB and {1} KB", _minFileSize,
                _maxFileSize);
        }
    }
}
