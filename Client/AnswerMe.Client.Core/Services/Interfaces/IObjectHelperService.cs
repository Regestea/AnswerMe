using Microsoft.AspNetCore.Components.Forms;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IObjectHelperService
{
    int CalculateChunkCount(double fileSizeMb);
    Task<byte[]> GetStreamChunkAsync(Stream inputStream, int numberOfChunks, int currentChunk);
    Task<List<byte[]>> ConvertStreamToChunksAsync(Stream inputStream, int numberOfChunks);
}