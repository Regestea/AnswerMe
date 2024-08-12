using System.ComponentModel.DataAnnotations;
using Models.Shared.Requests.Upload;

namespace Models.Shared.Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class RangeAllowed : ValidationAttribute
{
    private readonly string _fileTypePropertyName;

    public RangeAllowed(string fileTypePropertyName)
    {
        _fileTypePropertyName = fileTypePropertyName;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not double) return new ValidationResult($"Invalid value type");
        var property = validationContext.ObjectType.GetProperty(_fileTypePropertyName);
        if (property == null)
            return new ValidationResult($"Unknown property: {_fileTypePropertyName}");

        var dependentValue = property.GetValue(validationContext.ObjectInstance, null)?.ToString();
        
        if (!Enum.TryParse(dependentValue, true, out FileType fileType))
            return new ValidationResult($"Invalid file format");
        
        if (value is not double size) return new ValidationResult("Invalid value type");
        
        var validationField = fileType switch
        {
            FileType.audio => size is <= 0.01 or >= 100,
            FileType.image => size is <= 0.01 or >= 20,
            FileType.profile => size is <= 0.01 or >= 10,
            FileType.video => size is <= 0.01 or >= 2048,
            FileType.other => size is <= 0.01 or >= 2048,
            _ => true
        };
        return validationField ? new ValidationResult(GetErrorMessage(fileType)) : ValidationResult.Success;

    }

    private string GetErrorMessage(FileType fileType)
    {
        double maxFileSize = 0;
        double minFileSize = 0;
        switch (fileType)
        {
            case FileType.audio:
                maxFileSize = 100;
                minFileSize = 0.01;
                break;
            case FileType.image:
                maxFileSize = 20;
                minFileSize = 0.01;
                break;
            case FileType.video:
                maxFileSize = 2048;
                minFileSize = 0.01;
                break;
            case FileType.other:
                maxFileSize = 2048;
                minFileSize = 0.01;
                break;
            case FileType.profile:
                maxFileSize = 10;
                minFileSize = 0.01;
                break;
        }

        return $"Allowed file size is between {minFileSize} MB and {maxFileSize} MB";
    }
}