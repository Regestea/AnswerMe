using Models.Shared.RepositoriesResponseTypes;
using ObjectStorage.Api.Services.InterFaces;
using OneOf.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Blurhash.ImageSharp;

namespace ObjectStorage.Api.Services
{
    public class BlurHashService : IBlurHashService
    {
        public async Task<ReadResponse<bool>> ValidateBlurHash(string blurHash)
        {
            try
            {
                Blurhasher.Decode(blurHash, 1, 1, 1);
                return new Success<bool>(true);
            }
            catch (InvalidOperationException e)
            {
                return new Success<bool>(false);
            }
        }
    }
}
