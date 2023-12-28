using Models.Shared.OneOfTypes;

namespace Models.Shared.Responses.Shared;

public class ValidationErrorResponse
{
    public Dictionary<string, List<string>> Errors { get; set; }

    public ValidationErrorResponse(string field,string error)
    {
        Errors = new Dictionary<string, List<string>>()
        {
            [field]=new List<string>(){error}
        };
    }

    public ValidationErrorResponse(List<ValidationFailed> errorList)
    {
        Errors = new Dictionary<string, List<string>>();

        foreach (var validationFailed in errorList)
        {
            Errors.Add(validationFailed.Field, new List<string>(){validationFailed.Error});
        }
    }
}