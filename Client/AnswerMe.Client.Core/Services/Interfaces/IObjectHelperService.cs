using Microsoft.AspNetCore.Components.Forms;

namespace AnswerMe.Client.Core.Services.Interfaces;

public interface IObjectHelperService
{
    int CalculateChunkCount(double fileSizeMb);
    IAsyncEnumerable<Memory<byte>> GetStreamChunksAsync(Stream inputStream, int numberOfChunks);
}