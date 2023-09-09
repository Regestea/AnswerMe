using Models.Shared.RepositoriesResponseTypes;

namespace ObjectStorage.Api.Services.InterFaces;

public interface IBlurHashService
{
    Task<ReadResponse<bool>> ValidateBlurHash(string blurHash);
}