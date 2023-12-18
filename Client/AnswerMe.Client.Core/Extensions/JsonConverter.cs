using System.Text;
using System.Text.Json;
using Models.Shared.OneOfTypes;

namespace AnswerMe.Client.Core.Extensions
{
    public static class JsonConverter
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };

        public static Task<T> ToObject<T>(string content)
        {
            return Task.FromResult(JsonSerializer.Deserialize<T>(content, _options) ??
                                   throw new InvalidOperationException());
        }

        public static Task<StringContent> ToStringContent(object content)
        {
            return Task.FromResult(new StringContent(JsonSerializer.Serialize(content, _options), Encoding.UTF8,
                "application/json"));
        }

        public static Task<string> ToJson(object content)
        {
            return Task.FromResult(JsonSerializer.Serialize(content, _options));
        }

        public static async Task<List<ValidationFailed>> ToValidationFailedList(string content)
        {
            try
            {
                var errorDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(content);
                var validationFailedList = new List<ValidationFailed>();

                if (errorDict != null)
                    foreach (var error in errorDict)
                    {
                        foreach (string errorMessage in error.Value)
                        {
                            validationFailedList.Add(new ValidationFailed() { Field = error.Key, Error = errorMessage });
                        }
                    }

                return validationFailedList;
            }
            catch (Exception e)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(content);

                // Navigate to the "errors" field
                JsonElement errorsElement = jsonDocument.RootElement.GetProperty("errors");

                // Check if the "errors" field is an object
                if (errorsElement.ValueKind == JsonValueKind.Object)
                {
                    // Access the value of the "FileFormat" key
                    JsonElement fileFormatElement = errorsElement.GetProperty("FileFormat");

                    // Check if the "FileFormat" value is an array
                    if (fileFormatElement.ValueKind == JsonValueKind.Array)
                    {
                        // Access the first element of the array
                        JsonElement firstErrorElement = fileFormatElement.EnumerateArray().FirstOrDefault();

                        // Get the value as a string
                        string firstErrorValue = firstErrorElement.GetString();

                        return new List<ValidationFailed>() {new ValidationFailed(){Field = "error",Error = firstErrorValue} };
                    }
                }
            }

            return new List<ValidationFailed>() { new ValidationFailed(){Field = "error",Error = "some error happend"} };
        }
    }
}