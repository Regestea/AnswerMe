using System.Diagnostics;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Components.Forms;
using Blurhash.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace AnswerMe.Client.Core.Extensions;

public static class FileExtension
{
    public static async Task<string> GetPreviewUrlAsync(this IBrowserFile file,string fileFormat)
    {
        switch (fileFormat)
        {
            case "png" or "gif" or "jpg" or "jpeg":
            {
                var image = await file.RequestImageFileAsync("image/png,image/gif,image/jpg,image/jpeg", 400, 300);

                await using var imageStream = image.OpenReadStream(1024 * 1024 * 20);

                using MemoryStream ms = new();

                await imageStream.CopyToAsync(ms);


                return $"data:image/{fileFormat};base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            case "mp4" or "mov" or "mkv":
            {
                await using var imageStream = file.GetVideoThumbnailStream();

                using MemoryStream ms = new();

                await imageStream.CopyToAsync(ms);

                //convert stream to base64
                return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
            }
            default:
                return "";
        }
    }

    private static Stream GetVideoThumbnailStream(this IBrowserFile videoFile)
    {
        var tempThumbnailPath = Path.GetTempFileName();

        var ffmpegPath = "ffmpeg"; // Ensure ffmpeg is available in the system path or specify the full path to ffmpeg.exe

        // Create a temporary file for the thumbnail
        using (var ffmpegProcess = new Process())
        {
            ffmpegProcess.StartInfo.FileName = ffmpegPath;
            ffmpegProcess.StartInfo.Arguments = $"-i pipe:0 -ss 00:00:02 -vframes 1 -f image2 pipe:1";
            ffmpegProcess.StartInfo.RedirectStandardInput = true;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.CreateNoWindow = true;

            ffmpegProcess.Start();

            // Write the video file to the ffmpeg standard input
            using (var videoStream = videoFile.OpenReadStream(2000 * 1024 * 1024))
            {
                videoStream.CopyTo(ffmpegProcess.StandardInput.BaseStream);
            }
            ffmpegProcess.StandardInput.BaseStream.Close();

            // Read the thumbnail from ffmpeg standard output
            var thumbnailStream = new MemoryStream();
            ffmpegProcess.StandardOutput.BaseStream.CopyTo(thumbnailStream);
            thumbnailStream.Position = 0;

            // Ensure the process exits gracefully
            ffmpegProcess.WaitForExit();

            if (ffmpegProcess.ExitCode != 0)
            {
                throw new Exception($"FFmpeg error: {ffmpegProcess.StandardError.ReadToEnd()}");
            }
            return thumbnailStream;
        }

        
    }

    public static string ConvertBase64ToBlurHash(string base64ImageData)
    {
        // Extract the base64 string part
        var base64Data = base64ImageData.Split(',')[1];
        var imageBytes = Convert.FromBase64String(base64Data);

        using var image = Image.Load<Rgb24>(imageBytes);
        var blurHash = Blurhasher.Encode(image, 4, 3);
        return blurHash;
    }

    public static string ConvertBlurHashToBase64(string? blurHash)
    {
        if (string.IsNullOrWhiteSpace(blurHash))
        {
            blurHash = "L03l5Oj[fQj[offQfQfQfQfQfQfQ";
        }
        
        var image = Blurhasher.Decode(blurHash, 4, 3);
        using var ms = new MemoryStream();
        image.Save(ms, new PngEncoder());


        var imageBytes = ms.ToArray();
        return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
    }
}