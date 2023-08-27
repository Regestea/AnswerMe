using Models.Shared.DTOs.Error;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{

    public class CreateResponse<TResponse> : OneOfBase<Success<TResponse>, List<ValidationFailedDto>, Error<string>>
    {
        protected CreateResponse(OneOf<Success<TResponse>, List<ValidationFailedDto>, Error<string>> input)
            : base(input)
        {
        }

        public static implicit operator CreateResponse<TResponse>(Success<TResponse> _) => new(_);
        public static implicit operator CreateResponse<TResponse>(Error<string> _) => new(_);
        public static implicit operator CreateResponse<TResponse>(List<ValidationFailedDto> _) => new(_);
    }
}