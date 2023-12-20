using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnswerMe.Client.Core.DTOs.Response;
using Microsoft.AspNetCore.Components.Forms;
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
            var validationProblemDetails = JsonSerializer.Deserialize<ValidationDto>(content);
            var validationList = new List<ValidationFailed>();
            if (validationProblemDetails is { Errors: not null })
            {
                foreach (var error in validationProblemDetails.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error.Value.ToString()))
                    {
                        
                        var errorString = string.Join( " ",error.Value);
                        validationList.Add(new ValidationFailed(){Field = error.Key,Error = errorString});
                    }
                }
            }

            return validationList;
        }
    }
}