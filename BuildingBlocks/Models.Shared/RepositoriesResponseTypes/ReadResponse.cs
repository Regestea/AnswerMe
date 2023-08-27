using OneOf;
using OneOf.Types;

namespace Models.Shared.RepositoriesResponseTypes
{
    public class ReadResponse<TResponse> : OneOfBase<Success<TResponse>, NotFound>
    {
        protected ReadResponse(OneOf<Success<TResponse>, NotFound> input)
            : base(input)
        {
        }

        public static implicit operator ReadResponse<TResponse>(Success<TResponse> _) => new(_);
        public static implicit operator ReadResponse<TResponse>(NotFound _) => new(_);
    }
}