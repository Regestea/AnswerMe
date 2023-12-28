using AnswerMe.Client.Core.Services.Interfaces;
using Microsoft.AspNetCore.Components.Forms;

namespace AnswerMe.Client.Core.Services
{
    public class ObjectHelperService : IObjectHelperService
    {
        public int CalculateChunkCount(double fileSizeMb)
        {
            const int baseChunkSize = 10;
            const int maxChunkSize = 1000;

            // Calculate chunk count based on fileSizeMb
            int chunkCount = (int)Math.Ceiling(fileSizeMb / baseChunkSize) * baseChunkSize;

            // Ensure the chunk size does not exceed the maximum
            chunkCount = Math.Min(chunkCount, maxChunkSize);

            return chunkCount;
        }
        
        
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
        
            for (int i = 0; i < numberOfChunks; i++)
            {
                byte[] buffer = new byte[bufferSize];
                var bytesRead = await inputStream.ReadAtLeastAsync(buffer, buffer.Length, throwOnEndOfStream: false);

                Console.WriteLine("byte read "+ bytesRead);
                
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