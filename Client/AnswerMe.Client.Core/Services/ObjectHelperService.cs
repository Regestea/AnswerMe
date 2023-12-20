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

        // public  async Task<List<byte[]>> ConvertStreamToChunksAsync( Stream inputStream, int numberOfChunks)
        // {
        //     List<byte[]> chunks = new List<byte[]>();
        //     byte[] buffer = new byte[inputStream.Length / numberOfChunks]; // Calculate chunk size
        //
        //     for (int i = 0; i < numberOfChunks; i++)
        //     {
        //         int bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length);
        //         if (bytesRead > 0)
        //         {
        //             byte[] chunk = new byte[bytesRead];
        //             Array.Copy(buffer, chunk, bytesRead);
        //             chunks.Add(chunk);
        //         }
        //     }
        //
        //     return chunks;
        // }

        public async Task<List<byte[]>> ConvertStreamToChunksAsync(Stream inputStream, int numberOfChunks)
        {
            List<byte[]> chunks = new List<byte[]>();
            byte[] buffer = new byte[inputStream.Length / numberOfChunks]; // Calculate chunk size

            for (int i = 0; i < numberOfChunks; i++)
            {
                int bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    byte[] chunk = new byte[bytesRead];
                    Array.Copy(buffer, chunk, bytesRead);
                    chunks.Add(chunk);
                }
            }

            return chunks;
        }

        public async Task<byte[]> GetStreamChunkAsync(Stream inputStream, int numberOfChunks, int currentChunk)
        {
            if (numberOfChunks <= 0 || currentChunk < 0 || currentChunk >= numberOfChunks)
            {
                throw new ArgumentOutOfRangeException("Invalid numberOfChunks or currentChunk values");
            }

            int bufferSize = (int)Math.Ceiling((double)inputStream.Length / numberOfChunks);
            
            // int startPosition = currentChunk * bufferSize;
            
            long remainingBytes = Math.Min(bufferSize, inputStream.Length - inputStream.Position);
            
            byte[] buffer =new byte[remainingBytes];

            int bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                return buffer;
            }

            // Return null if no bytes were read (unexpected end of stream)
            return null;
        }
    }
}