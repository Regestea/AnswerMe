using System.Text;
using System.Text.Json;
using AnswerMe.Client.Core.DTOs.Error;

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

        public static Task<List<ValidationFailedDto>> ToValidationFailedList(string content)
        {
            var errorDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(content);
            var validationFailedList = new List<ValidationFailedDto>();

            if (errorDict != null)
                foreach (var error in errorDict)
                {
                    foreach (string errorMessage in error.Value)
                    {
                        validationFailedList.Add(new ValidationFailedDto() { Field = error.Key, Error = errorMessage });
                    }
                }

            return Task.FromResult(validationFailedList);
        }
    }
}