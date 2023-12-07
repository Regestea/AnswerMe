using Models.Shared.RepositoriesResponseTypes;

namespace ObjectStorage.Api.Services.InterFaces;

/// <summary>
/// Represents a blur hash service for validating blur hashes.
/// </summary>
public interface IBlurHashService
{
    /// <summary>
    /// Validates a BlurHash.
    /// </summary>
    /// <param name="blurHash">The BlurHash to be validated.</param>
    /// <returns>A Task that represents the asynchronous validation operation.
    /// The task result contains a ReadResponse object which wraps a boolean indicating
    /// whether the BlurHash is valid or not.</returns>
    Task<ReadResponse<bool>> ValidateBlurHash(string blurHash);
}