using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectStorage.Api.Test.DataConvertor
{
    public static class DataConvertor
    {
        public static async Task<List<byte[]>> ConvertStreamToChunksAsync(this Stream inputStream, int numberOfChunks)
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

    }
}
