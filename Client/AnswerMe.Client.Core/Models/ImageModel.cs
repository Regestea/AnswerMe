namespace AnswerMe.Client.Core.Models;

public class ImageModel
{
    public required string FileName { get; set; }
    public required string Url { get; set; }
    public required string BlurHashUrl { get; set; }
    public bool IsDownload { get; set; }
}