namespace AnswerMe.Infrastructure.Common.Extensions;

public static class BlobExtensions
{
    public static string? GetBlobEndpoint(this string? connectionString)
    {
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            var parts = connectionString.Split(';');
            var blobEndpointPart = parts.FirstOrDefault(part => part.StartsWith("BlobEndpoint="));
            if (blobEndpointPart != null)
            {
                var url = blobEndpointPart.Substring("BlobEndpoint=".Length);
                return url;
            }
        }
     
        return null;
    }
}