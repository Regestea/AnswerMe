namespace IdentityServer.Api.Extensions
{
    public static class StringNormalizer
    {
        public static string NormalizeString(this string content)
        {
            return content.ToLower().Trim();
        }
    }
}
