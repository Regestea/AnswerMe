using Microsoft.AspNetCore.Components.Forms;

namespace AnswerMe.Client.Core.Extensions;

public static class FIleExtension
{
    public static async Task<string> GetPreviewUrl(this IBrowserFile file)
    {
        if (file.ContentType is "image/png" or "image/gif" or "image/jpg" or "image/jpeg")
        {
            var image = await file.RequestImageFileAsync("image/png,image/gif,image/jpg,image/jpeg", 1920, 1080);

            using Stream imageStream = image.OpenReadStream(1024 * 1024 * 20);

            using MemoryStream ms = new();
            //copy imageStream to Memory stream
            await imageStream.CopyToAsync(ms);

            //convert stream to base64
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
        }

        return "";
    }
}