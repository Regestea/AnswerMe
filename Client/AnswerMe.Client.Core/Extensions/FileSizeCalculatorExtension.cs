namespace AnswerMe.Client.Core.Extensions
{
    public static class FileSizeCalculatorExtension
    {
        public static double SizeMB(this byte[] data)
        {
            return (double) data.Length / (1024.0 * 1024.0);
        }

        public static double SizeMB(this long size)
        {
            return (double) size / (1024.0 * 1024.0);
        }
    }
}
