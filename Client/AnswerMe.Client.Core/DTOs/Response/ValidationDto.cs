using System.Text.Json.Serialization;

namespace AnswerMe.Client.Core.DTOs.Response;

public class ValidationDto
{
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; set; }
}