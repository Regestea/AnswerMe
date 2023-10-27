using Models.Shared.OneOfTypes;
using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{

    public class CreateResponse<TResponse> : OneOfBase<Success<TResponse>, List<ValidationFailed>, ValidationFailed, Error<string>,AccessDenied,NotFound>
    {
        protected CreateResponse(OneOf<Success<TResponse>, List<ValidationFailed>, ValidationFailed, Error<string>, AccessDenied, NotFound> input)
            : base(input)
        {
        }

        public static implicit operator CreateResponse<TResponse>(Success<TResponse> _) => new(_);
        public static implicit operator CreateResponse<TResponse>(NotFound _) => new(_);
        public static implicit operator CreateResponse<TResponse>(Error<string> _) => new(_);
        public static implicit operator CreateResponse<TResponse>(AccessDenied _) => new(_);
        public static implicit operator CreateResponse<TResponse>(List<ValidationFailed> _) => new(_);
        public static implicit operator CreateResponse<TResponse>(ValidationFailed _) => new(_);
    }
}