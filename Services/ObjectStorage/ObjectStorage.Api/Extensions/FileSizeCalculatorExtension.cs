using System.Collections;

namespace ObjectStorage.Api.Extensions
{
    public static class FileSizeCalculatorExtension
    {
        public static double SizeMB(this byte[] data)
        {
            return (double) data.Length / (1024.0 * 1024.0);
        }
    }
}
