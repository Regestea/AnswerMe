using Microsoft.AspNetCore.Components.Forms;

namespace AnswerMe.Client.Core.Models;

public class VideoUploadModel
{
    public Guid Id { get; set; }
    public required string PreviewUrl { get; set; }
    public string? UploadToken { get; set; }
    public int? UploadProgress { get; set; }
    public bool IsDone { get; set; }
    public bool IsError { get; set; }
    public bool IsCanceled { get; set; }
    public required IBrowserFile File { get; set; }
}