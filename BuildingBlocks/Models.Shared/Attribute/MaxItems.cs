using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Models.Shared.Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class MaxItems : ValidationAttribute
{
    private readonly int _maxItems;

    public MaxItems(int maxItems)
    {
        _maxItems = maxItems;
        ErrorMessage = "The list cannot contain more than {0} items.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is IList list && list.Count > _maxItems)
        {
            var errorMessage = FormatErrorMessage(validationContext.DisplayName);
            return new ValidationResult(errorMessage);
        }

        return ValidationResult.Success;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(ErrorMessage ?? "The list cannot contain more than {0} items.", _maxItems);
    }
}