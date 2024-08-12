using Microsoft.AspNetCore.Components.Forms;

namespace AnswerMe.Client.Core.Models;

public class FileUploadModel
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public string? UploadToken { get; set; }
    public double FileSize { get; set; }
    public int? UploadProgress { get; set; }
    public bool IsDone { get; set; }
    public bool IsError { get; set; }
    public bool IsCanceled { get; set; }
    public required IBrowserFile File { get; set; }
}