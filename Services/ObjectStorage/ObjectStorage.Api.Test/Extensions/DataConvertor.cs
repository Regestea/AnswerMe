namespace ObjectStorage.Api.Test.Extensions
{
    public class DataConvertor
    {

        public async IAsyncEnumerable<Memory<byte>> GetStreamChunksAsync(Stream inputStream, int numberOfChunks)
        {
            if (numberOfChunks <= 0 || inputStream == null || !inputStream.CanRead)
            {
                throw new ArgumentException("Invalid arguments");
            }
        
            if (inputStream.Length <= 0)
            {
                // Handle streams with unknown or non-positive length
                throw new InvalidOperationException("Stream length is non-positive or unknown.");
            }
        
            int bufferSize = (int)Math.Ceiling((double)inputStream.Length / numberOfChunks);
            
            while (true)
            {
                byte[] buffer = new byte[bufferSize];
                var bytesRead = await inputStream.ReadAtLeastAsync(buffer, buffer.Length, throwOnEndOfStream: false);
                if (bytesRead > 0)
                {
                    yield return buffer.AsMemory(0, bytesRead);
                }
                else
                {
                    // Terminate the loop if there is nothing more to read
                    yield break;
                }
            }
          
        }
    }
}